using System.Collections.Generic;
using UnityEngine;

public class SmoothPath : MonoBehaviour
{
    public Vector3 mousePosition;

    public List<GameObject> sphereObject = new List<GameObject>();
    
    public List<Vector3> positions = new List<Vector3>();
    public List<Vector3> tempPositions = new List<Vector3>();

    [Range(1, 5)]public int calculateNumb = 5;
    [Range(0.1f, 1f)]public float intensity = 0.5f;

    public LineRenderer lineRenderer;
    private GameObject sphere;


    // Start is called before the first frame update
    void Start(){
        SphereCreateForPoints();
        lineRenderer = Instantiate(Resources.Load<GameObject>("Objects/Line")).GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            MousePositionUpdate();
            if(positions.Count >= 1){
                positions[positions.Count-1] = mousePosition;
                positions.Add(mousePosition);
            }
            else{
                InvokeRepeating("PreviewLine",0.1f,0.1f);
                positions.Add(mousePosition);
                positions.Add(mousePosition);
            }
            sphereObject.Add(Instantiate(sphere,mousePosition,Quaternion.identity)); 
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            MousePositionUpdate();
            positions[positions.Count-1] = mousePosition;
            sphereObject.Add(Instantiate(sphere,mousePosition,Quaternion.identity)); 
            PointsRecalculate(1,false);

            PointsClear();
            CancelInvoke("PreviewLine");
        }
    }

    private void PointsClear(){
        tempPositions.Clear();
        positions.Clear();
        sphereObject.Clear();
    }

    private void PointsRecalculate(int count,bool isPreview){
        tempPositions.Clear();

        if(count <= calculateNumb){ 
            for (int i = 0; i < positions.Count; i++) {
                if(i == 0){
                    tempPositions.Add(positions[i]);
                }
                if(i == positions.Count-1){
                    tempPositions.Add(positions[i]);
                }
                else if (intensity < 0.5f){
                    if(i != positions.Count-2)
                        tempPositions.Add((positions[i] + ((positions[i+1] - positions[i])*intensity)));
                    if( i != 0)
                        tempPositions.Add((positions[i+1] + ((positions[i] - positions[i+1])*intensity)));
                }
                else{
                    if( i != 0)
                        tempPositions.Add((positions[i+1] + ((positions[i] - positions[i+1])*intensity)));
                    if(i != positions.Count-2)
                        tempPositions.Add((positions[i] + ((positions[i+1] - positions[i])*intensity)));
                }
            }
            positions.Clear();
            positions.AddRange(tempPositions);
            PointsRecalculate((count+1),isPreview);
        }
        else if(isPreview){
            LinePreview();
        }
        else{
            NewLineRenderer();
        }
    }

    public void LinePreview(){
        
        lineRenderer.positionCount = positions.Count;
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            lineRenderer.SetPosition(i,positions[i]);
        }
    
        Vector3 tempVector = positions[positions.Count-1];
        positions.Clear();
        for (int i = 0; i <= sphereObject.Count; i++) {
            if(i != sphereObject.Count)
                positions.Add(sphereObject[i].transform.position);
            else{
                positions.Add(tempVector);
            }
        }
        
    }

    public void MousePositionUpdate(){
        mousePosition = Input.mousePosition;
        mousePosition.z = 10 ;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
    }
    
     public void PreviewLine(){
        if(sphereObject.Count >=1 ){
            if(Input.GetKey(KeyCode.Mouse1))MousePositionUpdate();
            positions[positions.Count-1] = mousePosition;
            PointsRecalculate(1,true);
        }
    }

    public void NewLineRenderer(){
        GameObject newLine = Instantiate(Resources.Load<GameObject>("Objects/Line"));
        LineRenderer lineRender = newLine.GetComponent<LineRenderer>();
        lineRender.positionCount = positions.Count;
        for (int i = 0; i < lineRender.positionCount; i++) {
            lineRender.SetPosition(i,positions[i]);
        }
    }

    public void SphereCreateForPoints(){
        sphere = Resources.Load<GameObject>("Objects/Sphere");
    }

   
}