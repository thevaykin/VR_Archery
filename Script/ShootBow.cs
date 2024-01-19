using System.Collections;
using UnityEngine;

public class ShootBow : MonoBehaviour {

    [SerializeField] GameObject arrowPrefab = null; 
    [SerializeField] GameObject bow = null;
    [SerializeField] int arrowsRemaining = 10; //сколько стрел есть у игрока
    [SerializeField] int pullSpeed = 10; //скорость стрелы

    private GameObject arrow; //созданная стрелка
    private TrailRenderer trail; //трассер

    private bool reset = false;

    private bool knockedArrow = false;
    private float drawDistance = 0;

    private Quaternion originalRot;

    private float changeRot = 45f / 46f;
    private float totalHipRotChange = -85f;

    private Vector3 changeBowPos = new Vector3(0, 0.005f * 2f, -0.0025f * 2f);
    private Vector3 changeArrowPos = new Vector3(-0.01651724137f*2f, -0.00655172413f*2f, 0.00125862068f*2f);

    private Vector3 totalBowPosChanges = new Vector3(0, 0, 0);
    private Vector3 totalArrowPosChanges = new Vector3(0, 0, 0);

    private void Start() {
        originalRot = bow.transform.localRotation; //первоначальное вращение лука
        SpawnArrow(); //респ стрелы, как только запускается сцена
    }

    //Один раз за кадр проверяйтся, запуск стрелы 
    private void Update() {
        ShootArrow();

        if (reset == true) {
            if (bow.transform.localRotation.eulerAngles.x <= originalRot.eulerAngles.x) reset = false;
            else StartCoroutine(ResetRotation());
        }
    }

    //Респ стрелы
    private void SpawnArrow() {
        if(arrowsRemaining > 0) { //если игрок имеет стреллы

            if (reset) { //Если лук все еще сбрасывается, то мы просто вернемся к начальной анимации
                reset = false;
                bow.transform.localRotation = originalRot;
            }

            knockedArrow = true;
            arrow = Instantiate(arrowPrefab, transform.position, transform.rotation) as GameObject; //create new arrow
            arrow.transform.SetParent(transform, true); //сетаем родительский элемент стрелы
            arrow.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            trail = arrow.GetComponent<TrailRenderer>(); //новый трасер
        }
    }
    
    //Плавный возврат лука обратно
    private IEnumerator ResetRotation() {
        while (bow.transform.localRotation.eulerAngles.x > originalRot.eulerAngles.x) {
            bow.transform.Rotate(Time.deltaTime * -5, 0, 0, Space.Self);

            if (bow.transform.localPosition.y >= -0.4) bow.transform.localPosition = new Vector3(0, bow.transform.localPosition.y - 0.002f, bow.transform.localPosition.z);

            yield return new WaitForSeconds(0.001f);
        }
    }

    //Сброс лука и стрелы после выстрела
    private void ResetBow() {
        totalBowPosChanges.y = 0;
        bow.transform.localPosition -= totalBowPosChanges; //сброс лука
        reset = true;

        transform.localPosition -= totalArrowPosChanges; //сброс стрелы

        totalBowPosChanges = new Vector3(0, 0, 0); //отчистка
        totalArrowPosChanges = new Vector3(0, 0, 0); //отчистка
        totalHipRotChange = -85f;
    }

    //Запуск стрелы
    private void ShootArrow() {
        if (arrowsRemaining > 0) { //если стрелы всё еще имеются         

            //получение сетки для рендера лука и стрелы
            SkinnedMeshRenderer bowSkin = bow.transform.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer arrowSkin = arrow.transform.GetComponent<SkinnedMeshRenderer>();

            Rigidbody arrowRB = arrow.transform.GetComponent<Rigidbody>();
            ArrowForce af = arrow.transform.GetComponent<ArrowForce>();

            //Нажатие на левую клавишу мыши
            if (Input.GetMouseButton(0)) {
        
            
                drawDistance += Time.deltaTime * pullSpeed; //установка расстояние рисования

                //Пока мы не достигнем полного расстояния натягивания, мы будем изменяем положение лука по мере его оттягивания назад
                if (drawDistance < 100) {
                    //перемещаем лук и стрелы так, чтобы они были ближе, когда игрок натягивает тетиву обратно
                    bow.transform.localPosition += changeBowPos; transform.localPosition += changeArrowPos;
                    //изменяем вращение лука по мере того, как они его натягивают
                    bow.transform.localRotation = Quaternion.Euler(totalHipRotChange += changeRot, -90, 0);
                    //увеличиваем общее количество изменений, чтобы мы могли отменить их после выстрела из лука
                    totalBowPosChanges += changeBowPos; totalArrowPosChanges += changeArrowPos;
                }
                else drawDistance = 100; //держим дистанцию вытягивания на уровне или ниже 100
                
            }

            //когда отпустили левую клавишу мыши
            if (Input.GetMouseButtonUp(0)) {
                if (drawDistance >= 10) { //если расстояние вытягивания достаточно для реального запуска стрелы
                    knockedArrow = false; 
                    arrowRB.isKinematic = false;

                    arrow.transform.SetParent(null);
                    arrow.transform.position = transform.position; //переопределяем позизцию

                    arrowsRemaining -= 1;

                    af.shootForce = af.shootForce * ((drawDistance / 100) + 0.05f); //расчёт силы воздействия стрелы на основе расстояния вытягивания

                    drawDistance = 0; //расстояние вытягивание на 0
                    af.enabled = true; //запускаем некст скрипт
                    trail.enabled = true; //запускаем трасер

                    ResetBow();
                }
                else {
                    drawDistance = 0;
                    ResetBow();
                }
            }
            // для визуального изменения лука и отображения оттягивания тетевы лука
            bowSkin.SetBlendShapeWeight(0, drawDistance); 
            arrowSkin.SetBlendShapeWeight(0, drawDistance);
        }

        //на всякий случай если сделали клик, а не mousedown и mouseup последовательно
        if (Input.GetMouseButtonDown(0) && !knockedArrow) SpawnArrow();        
    }
}
