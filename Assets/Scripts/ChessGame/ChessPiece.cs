using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    NONE,
    PAWN,
    ROOK,
    KNIGHT,
    BISHOP,
    QUEEN,
    KING
}

public enum Team
{
    BLACK,
    WHITE
}

public class ChessPiece : MonoBehaviour
{
    public Team team;
    public ChessPieceType type;

    public Vector3 currentPosition;
    public Vector3 currentLocalScale;

    private Vector3 desiredPosition;
    private Vector3 desiredScale;

    #region Unity Builtin Methods
    void Awake()
    {
        this.currentPosition = new Vector3(transform.position.x,0, transform.position.z);
        this.desiredPosition = this.currentPosition;

        this.currentLocalScale = this.transform.localScale;
        this.desiredScale = this.currentLocalScale;
    }

    public void Update()
    {
        transform.position = Vector3.Lerp(this.transform.position, this.desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(this.transform.localScale, this.desiredScale, Time.deltaTime * 10);
    }
    #endregion

    /** Sets this chessPieces desired Position
     * 
     */ 
    public virtual void SetPosition(Vector3 pos)
    {
        this.desiredPosition = pos;
        //--- Black King is poorly imported has to be centered ---
        if (this.type == ChessPieceType.KING && this.team == Team.BLACK)
        {
            this.desiredPosition.x += 0.5f;
            this.desiredPosition.z += 0.4f;
        }
        else
        {
            this.desiredPosition.x += 0.5f;
            this.desiredPosition.z += 0.5f;
        }
    }

    /** Sets this chessPieces desired localScale
     * 
     */
    public virtual void SetScale(float scale)
    {
        Vector3 newScale = Vector3.one * scale;
        this.desiredScale = newScale;
    }
}
