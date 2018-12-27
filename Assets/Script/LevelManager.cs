using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }

    // Debug
    public bool SHOW_COLLIDER = true;

    // Const
    private const float DISTANCE_BEFORE_SPAWN = 100.0f;
    private const int INITIAL_SEGMENTS = 10;
    private const int INITIAL_TRANSITION_SEGMENTS = 1;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;

    // List of Segments
    [HideInInspector]
    public List<Segment> segments = new List<Segment>();
    public List<Segment> availiableSegments = new List<Segment>();
    public List<Segment> availiableTransitions = new List<Segment>();

    // List of Pieces
    [HideInInspector]
    public List<Piece> piecesInPool = new List<Piece>();
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longBlocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();

    // Level Spawning
    private Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continiousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    // Gameplay
    private bool isMoving = false;

    private void Awake()
    {
        Instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
    }
    private void Start()
    {
        // Generate Segemnt
        for (int i = 0; i < INITIAL_SEGMENTS; i++)
        {
            if (i < INITIAL_TRANSITION_SEGMENTS)
                SpawnTransition();
            else
                GenerateSegment();
        }
    }
    private void Update()
    {
        // Generate More Segment
        if (currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
            GenerateSegment();

        if(amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN)
        {   
            segments[amountOfActiveSegments - 1].DeSpawn();
            amountOfActiveSegments--;
        }
    }

    private void GenerateSegment()
    {
        SpawnSegment();

        if (Random.Range(0f, 1f) < (continiousSegments * 0.25f))
        {
            // Spawn transition Segment
            continiousSegments = 0;
            SpawnTransition();
        }
        else
        {
            continiousSegments++;
        }
    }

    private void SpawnSegment()
    {
        List<Segment> possibleSegment = availiableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSegment.Count);

        Segment s = GetSegment(id, false);
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }
    private void SpawnTransition()
    {
        List<Segment> possibleTransition = availiableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    public Segment GetSegment(int id, bool transition)
    {
        Segment s = null;
        s = segments.Find(x => x.SegID == id && x.transition == transition && !x.gameObject.activeSelf);
        if (s == null)
        {
            GameObject go = Instantiate(transition ? availiableTransitions[id].gameObject : availiableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();

            s.SegID = id;
            s.transition = transition;

            segments.Insert(0, s);
        }
        else
        {
            segments.Remove(s);
            segments.Insert(0, s);
        }
        return s;
    }
    public Piece GetPiece(PieceType type, int indexPiece)
    {
        Piece p = piecesInPool.Find(x => x.type == type && x.indexPiece == indexPiece && !x.gameObject.activeSelf);

        if (p == null)
        {
            GameObject go = null;
            if (type == PieceType.ramp)
                go = ramps[indexPiece].gameObject;
            else if (type == PieceType.longblock)
                go = longBlocks[indexPiece].gameObject;
            else if (type == PieceType.jump)
                go = jumps[indexPiece].gameObject;
            else if (type == PieceType.slide)
                go = slides[indexPiece].gameObject;

            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            piecesInPool.Add(p);
        }

        return p;
    }
}
