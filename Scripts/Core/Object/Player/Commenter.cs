using System.Reflection;
using GameJam_URA.UI;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class Commenter : MonoBehaviour
    {
        const float CoolTime = 10f;
        const float StopDuration = 1f;

        [Parent]
        URA_PlayerReferenceHub hub;

        float _lastSayTime = float.NegativeInfinity;
        bool IsOnCoolTime => Time.time - _lastSayTime < CoolTime;

        float savedBaseSpeed;
        float stopTimer;
        bool isStopped;

        static readonly FieldInfo baseSpeedField =
            typeof(Walker).GetField("baseSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

        public void OpenCommentView()
        {
            if (IsOnCoolTime) return;
            UIViewHub.Instance.Comment.SetActive(true);
        }

        public bool TrySay(string comment)
        {
            if (IsOnCoolTime) return false;

            var stage = GameManager.Instance.CurrentStage;

            foreach (var task in stage.Normas)
            {
                if (task is ICommentItem ci && ci.Name == comment)
                    task.Complete();
            }
            foreach (var task in stage.Dobons)
            {
                if (task is ICommentItem ci && ci.Name == comment)
                    task.Complete();
            }

            UIViewHub.Instance.SpeechBubble.Show(transform, comment);
            _lastSayTime = Time.time;
            BeginStop();
            return true;
        }

        void BeginStop()
        {
            var mover = hub.PlayerHub.MoverReference;
            if (mover is not Walker walker) return;

            savedBaseSpeed = (float)baseSpeedField.GetValue(walker);
            baseSpeedField.SetValue(walker, 0f);
            stopTimer = StopDuration;
            isStopped = true;
        }

        void Update()
        {
            if (!isStopped) return;

            stopTimer -= Time.deltaTime;
            if (stopTimer > 0f) return;

            var mover = hub.PlayerHub.MoverReference;
            if (mover is Walker walker)
                baseSpeedField.SetValue(walker, savedBaseSpeed);

            isStopped = false;
        }
    }
}
