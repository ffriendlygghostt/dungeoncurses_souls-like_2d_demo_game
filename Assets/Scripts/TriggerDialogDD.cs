using UnityEngine;
using TriggerNamespace; // Подключаем пространство имен с TriggerHandler

public class TriggerDialogDD : MonoBehaviour
{
    private void Start()
    {
        // Проверка на наличие компонента TriggerHandler и добавление его, если он отсутствует
        if (GetComponent<TriggerHandler>() == null)
        {
            gameObject.AddComponent<TriggerHandler>();
        }
    }
}
