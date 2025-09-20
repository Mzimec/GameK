using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOperationManager : ScriptableSingleton<GridOperationManager> {
    private Grid grid;

    private void EnsureGrid() {
        if (grid == null) {
            grid = FindFirstObjectByType<Grid>();
            if (grid == null) {
                Debug.LogError("No Grid found in the scene.");
            }
        }
    }

    /// <summary>
    /// Vrátí všechny tile pozice v radiusu (Eukleidovská vzdálenost = kruh).
    /// </summary>
    public IEnumerable<Vector3Int> GetTilesInRadius(Vector3Int center, int radius) {
        EnsureGrid();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                Vector3Int offset = new Vector3Int(x, y, 0);
                if (offset.magnitude <= radius) {
                    yield return center + offset;
                }
            }
        }
    }

    /// <summary>
    /// Vrátí všechny tile pozice v čtvercové oblasti (radius na X a Y).
    /// </summary>
    public IEnumerable<Vector3Int> GetTilesInSquare(Vector3Int center, int radius) {
        EnsureGrid();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                yield return center + new Vector3Int(x, y, 0);
            }
        }
    }

    /// <summary>
    /// Vrátí všechny tile pozice podle Manhattan distance (diamant).
    /// </summary>
    public IEnumerable<Vector3Int> GetTilesInDiamond(Vector3Int center, int radius) {
        EnsureGrid();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= radius) {
                    yield return center + new Vector3Int(x, y, 0);
                }
            }
        }
    }

    /// <summary>
    /// Vrátí všechny tile pozice v přímce (např. pro laser, šíp apod.).
    /// </summary>
    public IEnumerable<Vector3Int> GetTilesInLine(Vector3Int start, Vector3Int direction, int length) {
        EnsureGrid();
        for (int i = 1; i <= length; i++) {
            yield return start + direction * i;
        }
    }
}