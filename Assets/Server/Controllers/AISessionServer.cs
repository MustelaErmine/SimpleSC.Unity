using SimpleSC.Server.Data;
using SimpleSC.Server.Actions;
using SimpleSC.Server.Units;
using SimpleSC.Server.Effects;
using System;
using UnityEngine.Events;

namespace SimpleSC.Server.Controllers
{
    internal class AISessionServer : SessionServer
    {
        int room_id;

        PlayerUnit playerUnit;
        AIEnemy aiEnemy;
        
        AISessionState CurrentState { 
            get
            {
                AISessionState state = new AISessionState();
                state.health = playerUnit.Health;
                state.enemyHealth = aiEnemy.Health;
                state.actions = playerUnit.PossibleActions.ToArray();
                state.effects = playerUnit.Effects.ToArray();
                return state;
            } 
        }

        int step = 0;

        protected override void InitConnection()
        {
            room_id = new Random().Next();

            playerUnit = new PlayerUnit();
            playerUnit.SessionServer = this;

            aiEnemy = new AIEnemy();
            aiEnemy.SessionServer = this;
            aiEnemy.player = playerUnit;

            InitOns();
            print("AI session server started;");
            SendResponse();
        }
        public void SendResponse()
        {
            Socket!.Emit("session_response", CurrentState);
            print("Initial state sent;");
        }
        public void InitOns()
        {
            UpService("action", ((object type) =>
            {
                string action_type = (string)type;
                print($"Action recieved {action_type};");
                playerUnit.ActivateAction(action_type, aiEnemy);
                NextStep();
            }));
        }
        public override void NextStep()
        {
            step++;
            if (playerUnit.Health <= 0)
                SendWin(false);
            else if (aiEnemy.Health <= 0)
                SendWin(true);
            else
            {
                if ((step & 1) == 0)
                {
                    Socket.Emit("state", CurrentState);
                    print("State sent;");
                    playerUnit.NotifyStep();
                }
                else
                {
                    aiEnemy.NotifyStep();
                }
            }
        }
        void UpService(string name, UnityAction<object> service)
        {
            Socket.AddListener(name, service);
        }
        public void SendWin(bool win)
        {
            print("Game ended;");
            Socket.Emit("end_game", win);
            StopServer();
        }
    }
}
