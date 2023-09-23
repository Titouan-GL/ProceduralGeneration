using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddProject : MonoBehaviour
{
    private int nbrOfProjects = 0;
    [SerializeField] GameObject projectPrefab;
    ChangeEverySize ces;

    private void Awake()
    {
        ces = GetComponentInParent<ChangeEverySize>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Project"))
        {
            GameObject go = Instantiate(projectPrefab);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(this.GetComponent<RectTransform>(), false);
            ProjectSize ps = go.GetComponent<ProjectSize>();
            ps.SetInitialPosition(new Vector2(0f, (rt.sizeDelta.y / 2 * -1) - rt.sizeDelta.y * nbrOfProjects));
            ces.UI.Add(ps);
            nbrOfProjects++;
        }
    }
}
