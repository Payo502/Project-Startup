using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObjectController : MonoBehaviour
{
    [SerializeField] private GameObject originalObject;
    [SerializeField] private List<GameObject> brokenPieces;
    [SerializeField] private float proximityThreshold = 2.0f; 
    [SerializeField] private float rebuildPercentage = 0.8f;

    private void Update()
    {
        int closePiecesCount = 0;
        foreach (var piece in brokenPieces)
        {
            if (IsCloseToOrigin(piece.transform))
            {
                closePiecesCount++;
            }
        }

        if ((float)closePiecesCount / brokenPieces.Count >= rebuildPercentage)
        {
            RebuildObject();
        }
    }

    private bool IsCloseToOrigin(Transform pieceTransform)
    {
        // Check distance from the original position (you may need to adjust this logic)
        return Vector3.Distance(pieceTransform.position, originalObject.transform.position) <= proximityThreshold;
    }

    private void RebuildObject()
    {
        // Activate the original object
        originalObject.SetActive(true);

        // Deactivate and/or destroy broken pieces
        foreach (var piece in brokenPieces)
        {
            piece.SetActive(false);
            Destroy(piece);
        }

        // Deactivate this controller
        this.enabled = false;
    }
}
