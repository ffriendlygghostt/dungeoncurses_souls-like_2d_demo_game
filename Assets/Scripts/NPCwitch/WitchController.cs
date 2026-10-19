//using UnityEngine;

//public class WitchControlle : MonoBehaviour
//{
//    public Transform player; // Ссылка на игрока
//    private SpriteRenderer spriteRenderer;

//    void Start()
//    {
//        spriteRenderer = GetComponent<SpriteRenderer>();
//    }

//    void Update()
//    {
//        // Проверяем, если игрок установлен
//        if (player != null)
//        {
//            // Получаем вектор от ведьмы к игроку
//            Vector3 direction = player.position - transform.position;

//            // Определяем угол поворота
//            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

//            // Флипуем спрайт в зависимости от направления
//            if (direction.x < 0)
//            {
//                spriteRenderer.flipX = true; // Флип по X, если игрок слева
//            }
//            else
//            {
//                spriteRenderer.flipX = false; // Без флипа, если игрок справа
//            }
//        }
//    }
//}
