using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; set; }

    // List of Pieces
    public List<Piece> piecesInPool = new List<Piece>();
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longBlocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();

    // Level Spawning

    public Piece GetPiece(PieceType type, int indexPiece)
    {
        Piece p = piecesInPool.Find(x => x.type == type && x.indexPiece == indexPiece && !x.gameObject.activeSelf);

        if (p == null)
        {
            GameObject gameObject = null;
            if (type == PieceType.ramp)
                gameObject = ramps[indexPiece].gameObject;
            else if (type == PieceType.longblock)
                gameObject = longBlocks[indexPiece].gameObject;
            else if (type == PieceType.jump)
                gameObject = jumps[indexPiece].gameObject;
            else if (type == PieceType.slide)
                gameObject = slides[indexPiece].gameObject;

            gameObject = Instantiate(gameObject);
            p = gameObject.GetComponent<Piece>();
            piecesInPool.Add(p);
        }

        return p;
    }
}
