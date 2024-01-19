using UnityEngine;

public class ArrowForce : MonoBehaviour {
    
    private Rigidbody rb;
    public float shootForce = 2000;

    //Всякий раз, когда включен скрипт get
    private void OnEnable() {
        rb = GetComponent<Rigidbody>(); //получаем стрелу
        rb.velocity = Vector3.zero; //обнуление скорости
        ApplyForce(); //Приложить силу, чтобы стрела полетела
    }

    //Вызывается один раз за кадр, вращайте стрелку, пока она летит по воздуху.
    private void Update() { transform.right = Vector3.Slerp(transform.right, transform.GetComponent<Rigidbody>().velocity.normalized, Time.deltaTime); }

    //полет стрелы в том направлении, в котором находится игрок
    private void ApplyForce() { rb.AddRelativeForce(Vector3.right * shootForce); }
}
