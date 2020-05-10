using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField]
    AntFactory antFactory;
    [SerializeField]
    Camera camera;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);
            antFactory.CreateAnt(pos, Team.Red);
        }

    }
}
