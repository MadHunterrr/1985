using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour, IDamageable
{

    #region Declaration

    public bool IsDead
    {
        get
        {
            return Char.CurHealth <= 0;
        }
    }
    private Character Char;
    private Rigidbody Rig;
    private Vector3 vert;
    private Vector3 horiz;
    [SerializeField]
    private InventoryUI inventoryUI;
    public Weapon EquipedWeapon;



    //Physics
    [SerializeField]
    private bool isGround = true;
    //сохраняет предыдущее состояние(если вы пригнули, то здесь оттобразиться что перед этим вы стояли на земле или наоборот)
    private bool playerDown = false;
    public bool IsPlayable;
    public Vector3 Point1;
    public Vector3 Point2;

    public Camera Cam
    {
        get
        {
            return Camera.main;
        }
    }

    public bool Smooth = false;
    public float SmoothSensitivity = 10;

    //----------camera control------------------//
    [Header("Camera")]
    public float RotationSpeedX = 5;
    public float RotationSpeedY = 5;
    public float MinX;
    public float MaxX;

    public bool SmoothCamera = false;
    public float SmoothCameraSensitivity = 10;

    public Transform FirstPersonCameraPosition;

    private Quaternion CharRot;
    private Quaternion CameraRot;


    [Header("Fov камеры при движении")]
    [Tooltip("Скорость, с которой будет изменяться FOV")]
    public float FOVUpSpeed = 0.5f;
    public float FOVDownSpeed = 0.8f;
    public float FOVAIMSpeed = 0.8f;
    public float FOVBase = 60;
    public float FOVSprint = 70;
    public float FOVAIM = 50;

    [Header("Динамическое движение камеры(при падении, спринте и т.д.)")]
    public float CamMovSpeed = 0.5f;
    public float CamMovRange = 0.2f;
    private float offset;
    public AnimationCurve CameraSpeed = new AnimationCurve(new Keyframe(-2, 1f), new Keyframe(1, 0.5f), new Keyframe(0, 0.7f));

    [Header("Animation")]
    public Animator HandAnimator;
    public bool UseWeaponMass = false;
    Character.Movement PrevState;
    public float MouseAxis;
    public float HorizontalAxis;
    bool left = false, right = false;
    // public AnimationCurve AIMMove = new AnimationCurve(new Keyframe(-1, 0), new Keyframe(-0.5f, -0.5f), new Keyframe(0, 0), new Keyframe(0.5f, 0.5f), new Keyframe(1, 1));

    Coroutine coroutine;
    public float hor;
    Weapon.ForShoot fs;

    // интерфейсt
    [Header("UI")]
    public GameObject ActiveUI;
    [SerializeField]
    private GameObject AmmoUI;
    private Image HealthBar;
    private Image EnduranceBar;

    //dispersion
    [Header("Dispersion")]
    public float SpeedModification;
    public float JumpModification;
    public float BaseDispersion
    {
        get
        {
            return (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) ?
                (EquipedWeapon.cur_fire_dispersion / 0.014f * 30 + SpeedModification * 2 + JumpModification) :
                (EquipedWeapon.cur_fire_dispersion / 0.014f * 30 + JumpModification);
        }
    }

    [Header("TEST")]
    public Vector3 velocity;
    public float magnitude;
    public float fixedUpdate;
    public float update;
    #endregion

    #region UI
    void ChangeAIM()
    {
        EquipedWeapon.AIM.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BaseDispersion);
        EquipedWeapon.AIM.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BaseDispersion);
    }

    public void DrawHealth()
    {
        HealthBar.fillAmount = Char.CurHealth / Char.Health;
        
    }

    public void DrawAmmo()
    {
        Text tx = AmmoUI.GetComponentInChildren<Text>();
        tx.text = EquipedWeapon.CurMagSize + "/" + FindAllAmmo();
    }
    #endregion
    // Use this for initialization
    void Start()
    {
        AmmoUI = GameObject.FindGameObjectWithTag("AmmoUI");
        IsPlayable = true;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        Char = GetComponent<Character>();
        Rig = GetComponent<Rigidbody>();
        PlayerController pl = this;
        if (EquipedWeapon != null)
        {
            EquipedWeapon.Equip(ref pl);
        }
        CharRot = transform.localRotation;
        CameraRot = Cam.transform.localRotation;
        GameObject go = new GameObject("Inventory");
        go.AddComponent<Inventory>();
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.down * 300;
        Char.Invent = go.GetComponent<Inventory>();
        //fs - это делегат, хранящий в себе функции, которые нам понадоблятся для стрельбы, 
        //и вызываются один раз за стрельбу. Это отдача, отрисовки изменения патронов
        fs = Otdacha;
        fs += DrawAmmo;
        HealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
        EnduranceBar = GameObject.FindGameObjectWithTag("EnduranceBar").GetComponent<Image>();
        DrawAmmo();
    }

    private void FixedUpdate()
    {

        RaycastHit hit;

        isGround = Physics.SphereCast(transform.position - transform.up * 0.25f, 0.38f, Vector3.down, out hit, 0.38f);


        //---------START TEST BLOCK----------//
        velocity = Rig.velocity;
        magnitude = Rig.velocity.magnitude;
        fixedUpdate = Time.fixedDeltaTime;

        //---------END TEST BLOCK------------//

        //если мы на земле, то выполняем следющие действия
        if (isGround)
        {

            Movement();
            JumpModification = 0;
            if (Input.GetButtonDown("Jump"))
            {
                Rig.AddForce(transform.up * 1000 * 15, ForceMode.Force);
                JumpModification = 5;
                ChangeAIM();
            }

        }

        //Пускаем райкаст для проверки возможности взаемодействия с объектами
        RaycastHit CheckObject;
        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out CheckObject, 2))
        {
            if (CheckObject.collider.GetComponent<IActiveable>() != null)
            {
                IActiveable curFocusObject = CheckObject.collider.GetComponent<IActiveable>();
                Collider curFocusObjectGO = CheckObject.collider;
                if (curFocusObject.isReady)
                {
                    ActiveUI.SetActive(true);
                    if (curFocusObjectGO.tag == "Item")
                    {
                        ActiveUI.GetComponentInChildren<Text>().text = "E - взять";
                    }
                    else if (curFocusObjectGO.tag == "Human")
                    {
                        ActiveUI.GetComponentInChildren<Text>().text = "E - говорить";
                    }
                    else if (curFocusObjectGO.tag == "Doors")
                    {
                        ActiveUI.GetComponentInChildren<Text>().text = "E - Открыть";
                    }
                    if (Input.GetButtonDown("Active"))
                    {
                        curFocusObject.OnActive(Char.gameObject);
                    }
                }
                else if (ActiveUI.activeInHierarchy)
                {
                    ActiveUI.SetActive(false);
                }
            }
            else if (ActiveUI.activeInHierarchy)
            {
                ActiveUI.SetActive(false);
            }
        }
        else if (ActiveUI.activeInHierarchy)
        {
            ActiveUI.SetActive(false);
        }

    }
    private void Update()
    {
        InputCheck();
        if (IsPlayable)
        {
            Vector3 newCameraPosition;
            newCameraPosition = FirstPersonCameraPosition.localPosition;
            newCameraPosition.y = FirstPersonCameraPosition.localPosition.y - offset;
            newCameraPosition.x = FirstPersonCameraPosition.localPosition.x;
            newCameraPosition.z = FirstPersonCameraPosition.localPosition.z;

            Cam.transform.localPosition = newCameraPosition;
            if (!playerDown && isGround)
            {
                StartCoroutine(JumpCamera());
            }
            playerDown = isGround;

            update = Time.deltaTime;
            Rotation();
            PistolAnimation();
        }
    }

    /// <summary>
    /// Функция проверяет, есть ли в инвентаре патроны нужного калибра для перезарядки
    /// </summary>
    /// <returns>Возвращает количество патронов, которые будут заряжены в магазин</returns>
    int CheckAmmo()
    {
        foreach (var i in Char.Invent.Items)
        {

            if (i is Ammo)
            {
                Ammo tItem = (Ammo)i;//tempItem
                if (tItem.ammoType == EquipedWeapon.ammo)
                {
                    if (i.Count >= EquipedWeapon.MagSize - EquipedWeapon.CurMagSize)
                    {
                        i.Count -= (uint)(EquipedWeapon.MagSize - EquipedWeapon.CurMagSize);
                        return EquipedWeapon.MagSize - EquipedWeapon.CurMagSize;
                    }
                    else if (i.Count < EquipedWeapon.MagSize - EquipedWeapon.CurMagSize && i.Count > 0)
                    {
                        int temp = (int)i.Count;
                        i.Count = 0;
                        Char.Invent.Items.Remove(i);
                        //Destroy(i.gameObject); //падает производительность
                        return temp;
                    }
                }
            }

        }
        return 0;
    }
    /// <summary>
    /// Функция ищет количество патронов, которые есть в инвентаре и подходит для текущего оружия
    /// </summary>
    /// <returns>Количество патронов в инвентаре</returns>
    int FindAllAmmo()
    {
        foreach (var i in Char.Invent.Items)
        {
            if (i is Ammo)
            {
                Ammo tItem = (Ammo)i;//tempItem
                if (tItem.ammoType == EquipedWeapon.ammo)
                {
                    return (int)tItem.Count;
                }
            }
        }


        return 0;
    }

    #region Input and Movement
    void InputCheck()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (!inventoryUI.gameObject.activeInHierarchy)
            {
                inventoryUI.gameObject.SetActive(true);
                inventoryUI.Activate(Char.Invent);
                IsPlayable = false;
                //если прицеливаемся при открытии инвентаря, то переводим прицел в исходящее положение
                if (Char.isAIM == true)
                {
                    if (coroutine != null)
                        StopCoroutine(coroutine);
                    coroutine = StartCoroutine(AIMFOVDown());
                    Char.isAIM = false;
                    Char.CurrentMovement = PrevState;
                    EquipedWeapon.AIM.gameObject.SetActive(true);
                    HandAnimator.SetBool("IsAIM", false);
                }
            }
            else
            {
                inventoryUI.DeActivate();
                inventoryUI.gameObject.SetActive(false);
                IsPlayable = true;
            }
        }

        //---------------
        if (Input.GetButton("Fire1") && IsPlayable)
        {
            if (HandAnimator.GetBool("isReady") && EquipedWeapon.CurMagSize > 0)
            {
                HandAnimator.SetTrigger("Shoot");
                StartCoroutine(EquipedWeapon.Shoot(0.32f / (EquipedWeapon.SPM / 60), Char.isAIM, HorizontalAxis, ChangeAIM, fs));

            }
            else if (HandAnimator.GetBool("isReady") && EquipedWeapon.CurMagSize == 0)
            {
                int temp = CheckAmmo();
                if (temp > 0)
                {
                    HandAnimator.SetBool("isReady", false);
                    HandAnimator.SetTrigger("IsEmpty");
                    StartCoroutine(EquipedWeapon.Reload(temp, EquipedWeapon.ReloadSound.length, DrawAmmo));
                }

            }

        }
        if (Input.GetButtonDown("Fire2") && IsPlayable)
        {
            if (Char.CurrentMovement == Character.Movement.Run || Char.CurrentMovement == Character.Movement.Sprint)
            {
                PrevState = Char.CurrentMovement;
                Char.CurrentMovement = Character.Movement.Walk;
            }
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(AIMFOVUp());
            Char.isAIM = true;
            EquipedWeapon.AIM.gameObject.SetActive(false);
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(AIMFOVDown());
            Char.isAIM = false;
            Char.CurrentMovement = PrevState;
            EquipedWeapon.AIM.gameObject.SetActive(true);
        }
        else if (Input.GetButtonDown("Reload") && HandAnimator.GetBool("isReady") && EquipedWeapon.CurMagSize != EquipedWeapon.MagSize)
        {
            int temp = CheckAmmo();
            if (temp > 0)
            {
                HandAnimator.SetBool("isReady", false);
                HandAnimator.SetTrigger("IsEmpty");
                StartCoroutine(EquipedWeapon.Reload(temp, EquipedWeapon.ReloadSound.length, DrawAmmo));
            }
        }
        //-----------------------------------------------------
        if (isGround && IsPlayable)
        {

            //бежать мы сможем только тогда, когда мы не целимся
            if (!Char.isAIM)
            {
                if (Input.GetButtonDown("Sprint") && vert != Vector3.zero)
                {
                    if (coroutine != null)
                        StopCoroutine(coroutine);
                    Char.CurrentMovement = Character.Movement.Sprint;
                    coroutine = StartCoroutine(FOVUp());
                }
                else if (Input.GetButtonUp("Sprint"))
                {
                    if (coroutine != null)
                        StopCoroutine(coroutine);
                    Char.CurrentMovement = Character.Movement.Run;
                    coroutine = StartCoroutine(FOVDown());
                }
                else if (Input.GetButtonUp("Walk/Run"))
                {
                    if (Char.CurrentMovement == Character.Movement.Run)
                    {
                        Char.CurrentMovement = Character.Movement.Walk;
                    }
                    else if (Char.CurrentMovement == Character.Movement.Walk)
                    {
                        Char.CurrentMovement = Character.Movement.Run;
                    }
                }
            }

            if (Input.GetButtonDown("Crouch"))
            {

                if (Char.CurrentMovement == Character.Movement.Run || Char.CurrentMovement == Character.Movement.Walk)
                    Char.CurrentMovement = Character.Movement.Crouch;
                else if (Char.CurrentMovement == Character.Movement.Crouch && !Char.isAIM)
                    Char.CurrentMovement = Character.Movement.Run;
                else if (Char.CurrentMovement == Character.Movement.Crouch && Char.isAIM)
                    Char.CurrentMovement = Character.Movement.Walk;
                PrevState = Char.CurrentMovement;
            }
        }
    }

    void Movement()
    {
        vert = transform.forward * Input.GetAxis("Vertical");
        horiz = transform.right * Input.GetAxis("Horizontal");

        //проверка на перемещение по диагонали. По умолчанию прередвижение по диагонали быстрее всех остальных, 
        //так как использует два множителя на скорость  - и вертикальный и горизонтальный. Тогда как движение в стороны использует один множитель.
        //Поэтому введен множитель 0.7f для сбалансирования скорости
        if (vert != Vector3.zero && horiz != Vector3.zero)
        {
            switch (Char.CurrentMovement)
            {
                case Character.Movement.Run:
                    vert *= Char.Speed * 0.7f;
                    horiz *= Char.Speed * 0.7f;
                    break;
                case Character.Movement.Walk:
                    vert *= Char.Speed * 0.7f * Char.WalkMultiple;
                    horiz *= Char.Speed * 0.7f * Char.WalkMultiple;
                    break;
                case Character.Movement.Crouch:
                    vert *= Char.Speed * 0.7f * Char.CrouchMultiple;
                    horiz *= Char.Speed * 0.7f * Char.CrouchMultiple;
                    break;
                case Character.Movement.Sprint:
                    vert *= Char.Speed * 0.7f * Char.SprintMultiple;
                    horiz *= Char.Speed * 0.7f * Char.SprintMultiple;
                    break;
            }
        }
        else
        {
            switch (Char.CurrentMovement)
            {
                case Character.Movement.Run:
                    vert *= Char.Speed;
                    horiz *= Char.Speed;
                    break;
                case Character.Movement.Walk:
                    vert *= Char.Speed * Char.WalkMultiple;
                    horiz *= Char.Speed * Char.WalkMultiple;
                    break;
                case Character.Movement.Crouch:
                    vert *= Char.Speed * Char.CrouchMultiple;
                    horiz *= Char.Speed * Char.CrouchMultiple;
                    break;
                case Character.Movement.Sprint:
                    vert *= Char.Speed * Char.SprintMultiple;
                    horiz *= Char.Speed * Char.SprintMultiple;
                    break;
            }
        }
        if (Smooth)
        {
            Rig.velocity = Vector3.Lerp(Rig.velocity, vert + horiz, SmoothSensitivity * Time.deltaTime);
            SpeedModification = Rig.velocity.magnitude * 3;
        }
        else
        {
            Rig.velocity = vert + horiz;
            SpeedModification = Rig.velocity.magnitude * 3;
        }
        ChangeAIM();

    }
    #endregion

    #region Camera Dynamic Move
    void Otdacha()
    {
        CameraRot *= Quaternion.Euler(-EquipedWeapon.cam_dispersion, 0, 0);
        if (EquipedWeapon.cam_dispersion < EquipedWeapon.cam_max_angle)
            EquipedWeapon.cam_dispersion += EquipedWeapon.cam_dispersion_inc;

    }

    Quaternion ClampRotation(Quaternion rot)
    {
        rot.x /= rot.w;
        rot.y /= rot.w;
        rot.z /= rot.w;
        rot.w = 1;

        float Angle = 2 * Mathf.Rad2Deg * Mathf.Atan(rot.x);
        Angle = Mathf.Clamp(Angle, MinX, MaxX);

        rot.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * Angle);
        return rot;
    }
    public void Rotation()
    {
        float xRot = Input.GetAxis("Mouse Y") * RotationSpeedX;
        float yRot = Input.GetAxis("Mouse X") * RotationSpeedX;

        CharRot *= Quaternion.Euler(0, yRot, 0);
        CameraRot *= Quaternion.Euler(-xRot, 0, 0);

        CameraRot = ClampRotation(CameraRot);

        if (SmoothCamera)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, CharRot, SmoothCameraSensitivity * Time.deltaTime);
            Cam.transform.localRotation = Quaternion.Slerp(Cam.transform.localRotation, CameraRot, SmoothCameraSensitivity * Time.deltaTime);
        }
        else
        {
            transform.localRotation = CharRot;
            Cam.transform.localRotation = CameraRot;
        }

    }

    IEnumerator FOVUp()
    {
        while (Cam.fieldOfView < FOVSprint)
        {
            Cam.fieldOfView += FOVUpSpeed;
            yield return new WaitForFixedUpdate();
        }
        Cam.fieldOfView = FOVSprint;
    }

    IEnumerator FOVDown()
    {
        while (Cam.fieldOfView > FOVBase)
        {
            Cam.fieldOfView -= FOVDownSpeed;
            yield return new WaitForFixedUpdate();
        }
        Cam.fieldOfView = FOVBase;
    }

    IEnumerator JumpCamera()
    {
        float t = 0;
        while (t < CamMovSpeed)
        {
            offset = Mathf.Lerp(0, CamMovRange, t / CamMovSpeed * CameraSpeed.Evaluate(offset));
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        t = 0;
        while (t < CamMovSpeed)
        {
            offset = Mathf.Lerp(CamMovRange, 0, t / CamMovSpeed * CameraSpeed.Evaluate(offset));
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        offset = 0;
    }

    IEnumerator AIMFOVUp()
    {
        while (Cam.fieldOfView > FOVAIM)
        {
            Cam.fieldOfView -= FOVAIMSpeed;
            yield return new WaitForFixedUpdate();
        }
        Cam.fieldOfView = FOVAIM;
    }
    IEnumerator AIMFOVDown()
    {
        while (Cam.fieldOfView < FOVBase)
        {
            Cam.fieldOfView += FOVAIMSpeed;
            yield return new WaitForFixedUpdate();
        }
        Cam.fieldOfView = FOVBase;
    }
    #endregion

    #region Animation
    void PistolAnimation()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            MouseAxis = Mathf.Lerp(MouseAxis, Input.GetAxis("Mouse X"), Time.deltaTime);
            MouseAxis = Mathf.Clamp(MouseAxis, -1, 1);
        }
        else if (Input.GetAxis("Mouse X") == 0)
            MouseAxis = Mathf.Lerp(MouseAxis, 0, 2f * Time.deltaTime);

        HandAnimator.SetFloat("Input X", MouseAxis);

        if (Char.isAIM == true)
        {
            HandAnimator.SetBool("IsAIM", true);
        }
        else
        {
            HandAnimator.SetBool("IsAIM", false);
        }
        if (Char.isAIM)
        {
            hor = Input.GetAxis("Horizontal");
            if (Input.GetAxis("Horizontal") < 0 && !left)
            {
                right = false;
                if (HorizontalAxis <= -0.7f)
                    left = true;

                HorizontalAxis = Mathf.Lerp(HorizontalAxis, Input.GetAxis("Horizontal"), Time.deltaTime * 2);
                HandAnimator.SetFloat("Input AIM X", HorizontalAxis);
            }
            else if (Input.GetAxis("Horizontal") > 0 && !right)
            {
                left = false;
                if (HorizontalAxis >= 0.7f)
                    right = true;

                HorizontalAxis = Mathf.Lerp(HorizontalAxis, Input.GetAxis("Horizontal"), Time.deltaTime * 2);
                HandAnimator.SetFloat("Input AIM X", HorizontalAxis);
            }
            else if (Input.GetAxis("Horizontal") == 0)
            {
                right = false;
                left = false;
                HorizontalAxis = Mathf.Lerp(HorizontalAxis, 0, Time.deltaTime);
                HandAnimator.SetFloat("Input AIM X", HorizontalAxis);
            }
            else
            {
                HorizontalAxis = Mathf.Lerp(HorizontalAxis, 0, Time.deltaTime);
                HandAnimator.SetFloat("Input AIM X", HorizontalAxis);
            }

        }

    }
    #endregion
    #region IDamageable
    public void TakeDamage(float damage)
    {
        if (!IsDead)
        {
            Char.CurHealth -= damage - (damage * ((float)Char.TorsoArmor / 100));
            DrawHealth();
        }

        if (IsDead)
        {
            Destruction();
        }
    }

    public void TakeDamage()
    {
        Char.CurHealth -= 10;
        DrawHealth();
    }

    public void Destruction()
    {
        Char.CurHealth = 0;
        DrawHealth();
    }
    #endregion
}
