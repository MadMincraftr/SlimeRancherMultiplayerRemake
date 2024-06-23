using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.DebugConsole
{
    public class SRMPConsole : MonoBehaviour
    {
        ConsoleWindow console = new ConsoleWindow();
        ConsoleInput input = new ConsoleInput();

        //
        // Create console window, register callbacks
        //
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            console.Initialize();
            console.SetTitle("Slime Rancher");

            input.OnInputText += OnInputText;

            Application.logMessageReceived += Application_logMessageReceived;

            SRMP.Log("Console Started");
        }

        //
        // Update the input every frame
        // This gets new key input and calls the OnInputText callback
        //
        private void Update()
        {
            input.Update();
        }

        //
        // It's important to call console.ShutDown in OnDestroy
        // because compiling will error out in the editor if you don't
        // because we redirected output. This sets it back to normal.
        //
        private void OnDestroy()
        {
            console.Shutdown();
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            throw new NotImplementedException();
        }

        private void OnInputText(string input)
        {
            throw new NotImplementedException();
        }
    }
}
