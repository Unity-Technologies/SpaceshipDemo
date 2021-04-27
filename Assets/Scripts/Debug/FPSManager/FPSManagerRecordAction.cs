using GameplayIngredients;
using GameplayIngredients.Actions;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManagerRecordAction : ActionBase
{
    public enum RecordAction
    {
        StartRecord,
        StopRecord,
        PauseRecord,
        UnpauseRecord,
        AbortRecord
    }

    public RecordAction recordAction = RecordAction.StartRecord;
    public string fileName = "Benchmark";

    public override void Execute(GameObject instigator = null)
    {
        if (!Manager.Has<FPSManager>())
            return;

        switch (recordAction)
        {
            default:
            case RecordAction.StartRecord:
                Manager.Get<FPSManager>().Record(fileName);
                break;
            case RecordAction.StopRecord:
                Manager.Get<FPSManager>().EndRecord(false);
                break;
            case RecordAction.AbortRecord:
                Manager.Get<FPSManager>().EndRecord(true);
                break;
            case RecordAction.PauseRecord:
                Manager.Get<FPSManager>().recordingPaused = true;
                break;
            case RecordAction.UnpauseRecord:
                Manager.Get<FPSManager>().recordingPaused = false;
                break;

        }
    }
}
