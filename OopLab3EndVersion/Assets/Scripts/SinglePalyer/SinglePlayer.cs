using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeDll;
using System;

public class SinglePlayer : PlayerManager
{
    void Init(int startX, int startY, float currentX, float currentY, List<List<Point>> maze, FieldManager fieldManager)
    {
        base.start_x = startX;
        base.start_y = startY;
        base.maze = maze;
        base.fieldManager = fieldManager;

        palyerCurrentX = currentX;
        palyerCurrentY = currentY;
        InitialDoorCount = 3;

        Vector3 startPlayerPos = fieldManager.wallMap.CellToWorld(new Vector3Int(startX + 1, startY + 1, 0));

        startPlayerPos += new Vector3(0.5f, 0.5f, 0);
        fieldManager.plm.gameObject.GetComponent<Transform>().position = startPlayerPos;
    }

    protected override bool CheckIfWin()
    {
        return countDoorsOpen == InitialDoorCount;
    }

    public override void PlayerInit(FieldManager fieldManager)
    {
        Init(fieldManager.startX, start_y, fieldManager.startPlayerPos.x, fieldManager.startPlayerPos.y, fieldManager.maze, fieldManager);
    }
    protected override bool NotifyOpenedDoor(Vector3Int coords, int i)
    {
        keys.Remove(keys[i]);
        fieldManager.decorMap.SetTile(coords, null);
        countDoorsOpen++;
        return true;
    }

    protected override void SetResults()
    {

        DateTime dateTime = DateTime.UtcNow.Date;
        ResultStatConfig.Date = dateTime.ToString("d");
        ResultStatConfig.Name = FieldConfig.Name;
        ResultStatConfig.Result = isWin ? "Win" : "Lose";
        ResultStatConfig.Steps = controlManager.steps.ToString();
        ResultStatConfig.Time = spend_time.ToString();
        ResultStatConfig.Type = "SinglePlayer";

        StatisticHelper.WriteResult();
    }
}
