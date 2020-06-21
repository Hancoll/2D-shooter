using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class GradualLoadingMapManager : MonoBehaviour
{
    List<GameObject> ActiveObjects = new List<GameObject>();
    List<GradualLoadingObject> graduaLoadingObjects = new List<GradualLoadingObject>();
    Coroutine checkActiveObjects;
    float timeBeetwenCheckActiveObjects = 1;

    private void Awake()
    {
        graduaLoadingObjects.AddRange(FindObjectsOfType<GradualLoadingObject>());

        //Отключение всех объектов
        foreach (GradualLoadingObject graduaLoadingObject in graduaLoadingObjects)
            graduaLoadingObject.ChangeActive(false);

        checkActiveObjects = StartCoroutine(ICheckActiveObjects());
    }

    public void RemoveGraduaLoadingObjects(GradualLoadingObject graduaLoadingObject)
    {
        graduaLoadingObjects.Remove(graduaLoadingObject);
    }

    void CheckActiveObjects()
    {
        List<GradualLoadingObject> newGraduaLoadingObjects = new List<GradualLoadingObject>();
        List<GameObject> newActiveObjects = new List<GameObject>();

        newGraduaLoadingObjects.AddRange(FindObjectsOfType<GradualLoadingObject>());

        graduaLoadingObjects.AddRange(newGraduaLoadingObjects.Where(x => !graduaLoadingObjects.Contains(x)));

        foreach (GradualLoadingObject graduaLoadingObject in graduaLoadingObjects)
        {
            if (Vector2.Distance(Camera.main.transform.position, graduaLoadingObject.transform.position) > GameManager.LoadingDistance && graduaLoadingObject.IsActive)
            {
                graduaLoadingObject.ChangeActive(false);
                ActiveObjects.Remove(graduaLoadingObject.gameObject);
            }

            else if (Vector2.Distance(Camera.main.transform.position, graduaLoadingObject.transform.position) <= GameManager.LoadingDistance && !graduaLoadingObject.IsActive)
            {
                graduaLoadingObject.ChangeActive(true);
                ActiveObjects.Remove(graduaLoadingObject.gameObject);
                newActiveObjects.Add(graduaLoadingObject.gameObject);
            }

            ActiveObjects.AddRange(newActiveObjects);
        }
    }

    IEnumerator ICheckActiveObjects()
    {
        while (true)
        {
            CheckActiveObjects();
            yield return new WaitForSeconds(timeBeetwenCheckActiveObjects);
        }
    }
}
