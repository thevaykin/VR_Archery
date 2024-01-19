using UnityEngine;

public class EmbedArrow : MonoBehaviour {
    [SerializeField] GameObject sparksPrefab = null;
    private GameObject sparks;
    private Rigidbody rb;
    private bool sparkExists = false;

    //когда стрела будет создана, мы получим ее жесткое тело
    private void Start() { rb = GetComponent<Rigidbody>(); }

    private void Update() {
        //проверьте, существует ли объект искра, и (если да) проверьте, остановила ли созданная искра свою анимацию
        if (sparkExists && !sparks.GetComponent<ParticleSystem>().isEmitting) {
            //Если искра перестала испускаться, мы уничтожаем ее
            Destroy(sparks, 1f);
            //Установливаем false, поскольку объект искра больше не должен существовать
            sparkExists = false;            
        }
    }

    //Как только стрелка столкнется с сетчатым коллайдером, будет вызвана эта функция
    private void OnCollisionEnter(Collision col) {
        //игнорируйте объект player, а также другие объекты стрел
        if (col.gameObject.tag == "Arrow" || col.gameObject.tag == "Player") return; 

        transform.GetComponent<ArrowForce>().enabled = false; //Мы отключим сценарий принудительного запуска снаряда
        rb.isKinematic = true; //стоп перемещать объект

        //создаёт анимацию, которая показывает, что стрелка попала во что-то
        sparks = Instantiate(sparksPrefab, transform) as GameObject; //создайте экземпляр нового объекта sparks
        sparksPrefab.transform.rotation = transform.rotation; 
        sparkExists = true;

        transform.localScale += new Vector3(3, 3, 3); //увеличьте масштаб стрелки, так как ее трудно увидеть обычным способом

        transform.SetParent(GameObject.FindGameObjectWithTag("ArrowContainer").transform, true);
    }
}