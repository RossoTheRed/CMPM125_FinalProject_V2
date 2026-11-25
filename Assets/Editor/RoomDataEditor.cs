using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomData))]
public class RoomDataEditor : Editor
{
    //Editor based class to help create room designs within the editor

    public override void OnInspectorGUI()
    {
        RoomData room = (RoomData)target;

        EditorGUI.BeginChangeCheck();

        int newWidth = EditorGUILayout.IntField("Width", room.width);
        int newHeight = EditorGUILayout.IntField("Height", room.height);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(room, "Resize Room");

            room.width = Mathf.Max(1, newWidth);
            room.height = Mathf.Max(1, newHeight);

            room.tiles.RemoveAll(tile => tile.position.x < 0 || tile.position.x >= room.width || tile.position.y < 0 || tile.position.y >= room.height);

            EditorUtility.SetDirty(room);
        }

        EditorGUILayout.Space();

        for (int y = room.height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < room.width; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                RoomTileData tile = room.tiles.Find(t => t.position == position);

                string label = "";
                Color color = Color.gray;

                if (tile != null)
                {
                    if (tile.isDoor)
                    {
                        label = "D";
                        color = Color.green;
                    }
                    else
                    {
                        label = "R";
                        color = Color.cyan;
                    }
                }

                Color previousColor = GUI.backgroundColor;
                GUI.backgroundColor = color;

                if (GUILayout.Button(label, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    Undo.RecordObject(room, "Edit Room Tile");

                    if (tile == null)
                    {
                        room.tiles.Add(new RoomTileData{position = position, isDoor = false});
                    }
                    else if (!tile.isDoor)
                    {
                        tile.isDoor = true;
                    }
                    else
                    {
                        room.tiles.Remove(tile);
                    }

                    EditorUtility.SetDirty(room);
                }

                GUI.backgroundColor = previousColor;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}