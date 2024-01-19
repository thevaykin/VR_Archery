using UnityEngine;

public class AimCamera : MonoBehaviour {
    private Rigidbody rb;
    private bool crouch = false;
    private Vector3 ogScale;    
    [SerializeField] float verSensitivity = 2;
    [SerializeField] float horSensitivity = 2;
    [SerializeField] float speed = 0.5f;
    [SerializeField] float crouchScale = 1.5f;
    [SerializeField] Camera cam = null;

    //Вызывается при создании gameobject (который находится в начале игры)
    private void Start() {
        rb = GetComponent<Rigidbody>(); //получить компонент игрока
        Cursor.visible = false; //видимость курсора
        ogScale = transform.localScale;
    }

    private void Update() {
        //Проверяем, не хочет ли игрок переместить свою камеру
        CheckForCameraMovement();

        if (Input.GetKeyDown(KeyCode.LeftControl) && !crouch){
            transform.localScale -= new Vector3(0, transform.localScale.y - crouchScale, 0);
            crouch = true;
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl) && crouch){
            transform.localScale = ogScale;
            crouch = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Cursor.visible) Cursor.visible = false; //видимость курсора
            else Cursor.visible = true;
        }

        //Если игрок не целитеся то двигайтеся с обычной скоростью
        if(!Input.GetMouseButton(0) && !Input.GetKey(KeyCode.Space)) CheckForMovement(speed);
        //исли целится двигается с половиной скорости
        else CheckForMovement(speed / 1.5f);
    }

    //Когда игрок начнет двигаться, то тела=о перемещается
    private void CheckForMovement(float moveSpeed) {        
        rb.MovePosition(transform.position + (transform.right * Input.GetAxis("Vertical") * moveSpeed) 
            + (transform.forward * -Input.GetAxis("Horizontal") * moveSpeed));        
    }

    //крутит камерой
    private void CheckForCameraMovement() {
        float mouseX = Input.GetAxisRaw("Mouse X"); 
        float mouseY = Input.GetAxisRaw("Mouse Y"); 
        //вычисляем вращение по осям на основе входных данных
        Vector3 rotateX = new Vector3(mouseY * verSensitivity, 0, 0); 
        Vector3 rotateY = new Vector3(0, mouseX * horSensitivity, 0);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateY)); //поворот тела
        cam.transform.Rotate(-rotateX); //повернить камеру в обратном направлении по оси x (чтобы движение не было обратным).
}
}
