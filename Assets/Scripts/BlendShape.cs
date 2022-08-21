using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System.Linq;
using System;

public class BlendShape : MonoBehaviour
{

    public List<ShapeSlot> cornerShapesGameObjects;

    public List<SpriteShapeController> cornerShapes = new List<SpriteShapeController>();

    public ShapeSlot centerShapeGameObject;

    public SpriteShapeController centerShape;

    public SpriteShapeController currentShape;

    public List<SpriteShapeController> allShapes;

    public float speed = 5; 

    public SpriteShapeController targetShape;

    public Vector3 targetPoint;

    public DateTime lastTime;

    public int score;

    public Text scoreDisplay;

    public GameObject arrow;

    public GameObject point;

    public bool isSetup = false;

    public float closeness = 0.2f;

    public AudioSource audioSource;

    // Start is called before the first frame update
    public void Setup()
    {

        foreach (var shape in cornerShapesGameObjects)
        {
            cornerShapes.Add(shape.spriteShapeController);
        }

        centerShape = centerShapeGameObject.spriteShapeController;

        foreach (var shape in cornerShapes)
        {
            for (int i = 0; i < shape.spline.GetPointCount(); i++)
            {
                shape.spline.SetTangentMode(i, ShapeTangentMode.Broken);
            }
        }

        for (int i = 0; i < centerShape.spline.GetPointCount(); i++)
        {
            centerShape.spline.SetTangentMode(i, ShapeTangentMode.Broken);
        }

        for (int i = 0; i < centerShape.spline.GetPointCount(); i++)
        {
            centerShape.spline.SetPosition(i, cornerShapes.Average(s => s.spline.GetPosition(i)));
            centerShape.spline.SetLeftTangent(i, cornerShapes.Average(s => s.spline.GetLeftTangent(i)));
            centerShape.spline.SetRightTangent(i, cornerShapes.Average(s => s.spline.GetRightTangent(i)));

        }

        for (int i = 0; i < currentShape.spline.GetPointCount(); i++)
        {
            currentShape.spline.SetTangentMode(i, ShapeTangentMode.Broken);
        }


        for (int i = 0; i < targetShape.spline.GetPointCount(); i++)
        {
            targetShape.spline.SetTangentMode(i, ShapeTangentMode.Broken);
        }

        allShapes = cornerShapes.ToList();
        allShapes.Add(centerShape);

        lastTime = DateTime.Now;

        UpdateCurrentShape();
        UpdateTargetShape();


        isSetup = true;
    }

    public void UpdateCurrentShape()
    {
        var closest = allShapes.Where(s => (s.transform.position - currentShape.transform.position).magnitude < 5f).ToList();

        allShapes.ForEach(s => s.GetComponent<SpriteShapeRenderer>().color = Color.white);
        closest.ForEach(s => s.GetComponent<SpriteShapeRenderer>().color = Color.red);

        var closestDistance = closest.Select(s => Tuple.Create(s, 1 - (s.transform.position - currentShape.transform.position).magnitude / 5)).ToList();
        var totalDistance = closestDistance.Sum(s => s.Item2);

        for (int i = 0; i < currentShape.spline.GetPointCount(); i++)
        {
            currentShape.spline.SetPosition(i, closestDistance.Sum(s => s.Item1.spline.GetPosition(i) * s.Item2 / totalDistance));
            currentShape.spline.SetLeftTangent(i, closestDistance.Sum(s => s.Item1.spline.GetLeftTangent(i) * s.Item2 / totalDistance));
            currentShape.spline.SetRightTangent(i, closestDistance.Sum(s => s.Item1.spline.GetRightTangent(i) * s.Item2 / totalDistance));

        }

    }

    public void UpdateTargetShape()
    {
        targetPoint = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0);

        while (!IsPointInPolygon(targetPoint, cornerShapes.Select(c => c.transform.position).ToArray()))
        {
            targetPoint = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0);
        }

        //Instantiate(point, targetPoint, Quaternion.identity);

        var closest = allShapes.Where(s => (s.transform.position - targetPoint).magnitude < 5f).ToList();

        allShapes.ForEach(s => s.GetComponent<SpriteShapeRenderer>().color = Color.white);
        closest.ForEach(s => s.GetComponent<SpriteShapeRenderer>().color = Color.red);

        var closestDistance = closest.Select(s => Tuple.Create(s, 1 - (s.transform.position - targetPoint).magnitude / 5)).ToList();
        var totalDistance = closestDistance.Sum(s => s.Item2);

        for (int i = 0; i < targetShape.spline.GetPointCount(); i++)
        {
            targetShape.spline.SetPosition(i, closestDistance.Sum(s => s.Item1.spline.GetPosition(i) * s.Item2 / totalDistance));
            targetShape.spline.SetLeftTangent(i, closestDistance.Sum(s => s.Item1.spline.GetLeftTangent(i) * s.Item2 / totalDistance));
            targetShape.spline.SetRightTangent(i, closestDistance.Sum(s => s.Item1.spline.GetRightTangent(i) * s.Item2 / totalDistance));

        }

        Debug.Log("Yay");


    }

    public void FixedUpdate()
    {
        if (!isSetup)
        {
            return;
        }

        currentShape.transform.Translate(new Vector3( Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime, 0));

        UpdateCurrentShape();

        // arrow.transform.LookAt(targetPoint, Vector3.right);
        arrow.transform.right = targetPoint - arrow.transform.position;


        if ((currentShape.transform.position - targetPoint).magnitude < closeness)
        {
            audioSource.Play();

            UpdateTargetShape();

            var diff = DateTime.Now - lastTime;
            lastTime = DateTime.Now;

            score += (int)(Math.Max(20 - diff.TotalMilliseconds / 1000.0f, 0) * 200);

            scoreDisplay.text = score.ToString();
        }
    }


    public bool IsPointInPolygon(Vector3 point, Vector3[] polygon)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector3 endPoint = polygon[polygonLength - 1];
        endX = endPoint.x;
        endY = endPoint.y;
        while (i < polygonLength)
        {
            startX = endX; startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x; endY = endPoint.y;
            //
            inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }


}