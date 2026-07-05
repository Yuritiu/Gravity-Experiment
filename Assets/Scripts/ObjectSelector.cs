using UnityEngine;
using System.Collections;

public class GravitySelector : MonoBehaviour
{
    [Header("Setup")]
    public Camera cam;
    public float selectRange = 100f;
    public float rotateDuration = 0.3f;

    [Header("Highlight")]
    public Color highlightColor = Color.yellow;

    public static LocalGravity selectedObject;

    private Renderer selectedRenderer;
    private Color originalColor;

    void Update()
    {
        // Deselect automatically if object is destroyed or disabled
        if (selectedObject != null && !selectedObject.gameObject.activeInHierarchy)
        {
            DeselectCurrentObject();
        }

        HandleSelection();
        HandleGravityInput();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, selectRange))
            {
                LocalGravity hitObject = hit.collider.GetComponent<LocalGravity>();

                if (hitObject == selectedObject)
                {
                    // Clicked the same object → deselect
                    DeselectCurrentObject();
                }
                else
                {
                    // Clicked a different object
                    LocalGravity prev = selectedObject;
                    Renderer prevRend = selectedRenderer;
                    Color prevOriginal = originalColor;

                    DeselectCurrentObject(); // don’t reset nulls yet

                    if (prevRend != null)
                        StartCoroutine(LerpColor(prevRend, prevRend.material.color, prevOriginal, 0.2f));

                    if (hitObject != null)
                        SelectObject(hitObject);
                }
            }
            else
            {
                // Clicked empty space
                DeselectCurrentObject();
            }
        }
    }

    void SelectObject(LocalGravity obj)
    {
        selectedObject = obj;
        selectedRenderer = obj.GetComponent<Renderer>();
        if (selectedRenderer != null)
        {
            originalColor = selectedRenderer.material.color;
            StartCoroutine(LerpColor(selectedRenderer, originalColor, highlightColor, 0.2f));
        }
    }

    void DeselectCurrentObject()
    {
        if (selectedObject != null && selectedRenderer != null)
        {
            StartCoroutine(LerpColor(selectedRenderer,
                selectedRenderer.material.color,
                originalColor, 0.2f));
        }

        selectedObject = null;
        selectedRenderer = null;
    }


    void HandleGravityInput()
    {
        if (selectedObject == null) return;

        Transform camTransform = cam.transform;
        Vector3 newDir = selectedObject.gravityDirection;
        bool changed = false;

        if (Input.GetKeyDown(KeyCode.W)) { newDir = camTransform.forward; changed = true; }
        if (Input.GetKeyDown(KeyCode.S)) { newDir = -camTransform.forward; changed = true; }
        if (Input.GetKeyDown(KeyCode.A)) { newDir = -camTransform.right; changed = true; }
        if (Input.GetKeyDown(KeyCode.D)) { newDir = camTransform.right; changed = true; }
        if (Input.GetKeyDown(KeyCode.Q)) { newDir = -camTransform.up; changed = true; }
        if (Input.GetKeyDown(KeyCode.E)) { newDir = camTransform.up; changed = true; }

        if (changed)
            StartCoroutine(SmoothChangeGravity(selectedObject, newDir));
    }

    IEnumerator SmoothChangeGravity(LocalGravity obj, Vector3 newDir)
    {
        Quaternion startRot = obj.transform.rotation;
        Quaternion targetRot = Quaternion.FromToRotation(obj.transform.up, -newDir) * obj.transform.rotation;

        Vector3 startDir = obj.gravityDirection;
        Vector3 targetDir = newDir;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotateDuration;
            obj.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            obj.SetGravity(Vector3.Slerp(startDir, targetDir, t));
            yield return null;
        }

        obj.transform.rotation = targetRot;
        obj.SetGravity(newDir);
    }

    IEnumerator LerpColor(Renderer rend, Color start, Color end, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            if (rend != null)
                rend.material.color = Color.Lerp(start, end, t);
            yield return null;
        }
        if (rend != null)
            rend.material.color = end;
    }

    private void OnDisable()
    {
        if (selectedRenderer != null)
        {
            selectedRenderer.material.color = originalColor;
            selectedRenderer = null;
            selectedObject = null;
        }
    }
}
