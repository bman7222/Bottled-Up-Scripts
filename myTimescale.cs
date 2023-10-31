using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class frameAction
{
    [Space(2f)]
    [Header("Frame Action")]

    [Tooltip("The name of the frame action")]
    [SerializeField] private string name;

    [Tooltip("The Actions the to be performed")]
     private Action action;

    [Tooltip("The Action with Hit Data to be performed")]
     private Action<HitData> actionHitData;

    public frameAction()
    {
        this.action = null;
        this.name = null;
        this.actionHitData = null;
    }

    public frameAction(string name, Action action)
    {
        this.action = action;
        this.name = name;
        this.actionHitData = null;
    }

    public frameAction(string name, Action<HitData> actionHitData)
    {
        this.action = null;
        this.name = name;
        this.actionHitData = actionHitData;
    }

    public string getName()
    {
        return name;
    }

    public Action getAction()
    {
        return action;
    }

    public Action<HitData> getActionHitData()
    {
        return actionHitData;
    }


}

//Time scale 
//holds dict of timers 
    //timers 
    //can execute an action on start, on finish, or you can also give it a list of actions to execute. 


[System.Serializable]
public class timer
{
    [System.Serializable]
    public enum timerType
    {
        attackTimer,
        prepareAttackTimer,
        attackCooldownTimer,
        hitStopTimer,
        hitStunTimer,
        vibrateTimer,
        Null,
    }

    [Space(2f)]
    [Header("Timer")]

    [Tooltip("The name of the timer")]
    [SerializeField] string name;

    [Tooltip("Classification of the timer")]
    [SerializeField] timerType type;

    [Tooltip("The name of the timer to add, set, or decrease time remaining from")]
    [SerializeField] timer.timerType target;

    [Tooltip("Dictionary with frame to execute as key and action to execute as value")]
    private Dictionary<int, List<frameAction>> executionActions;

    [System.Serializable]
    public class dictionaryDisplay
    {
        [System.Serializable]
        public class frameActionDisplay
        {
            public List<frameAction> displayActions;
        }

        public List<int> frames;

        public List<frameActionDisplay> actions;
    }

    [Space(2f)]
    [Header("Current Time")]

    [Tooltip("The amount of time remaining on the timer.")]
    [SerializeField] float timeRemaining;

    [Tooltip("The current frame this timer is on")]
    [SerializeField] int currentFrame;

    [Tooltip("The amount of time that has elapsed from this timer")]
    [SerializeField] float timeElapsed;

    [Space(2f)]
    [Header("Actions to Execute Dictionary")]

    [Tooltip("Displays the Dictionary values")]
    [SerializeField] private dictionaryDisplay executionActionsDisplay;

    [Space(2f)]
    [Header("Timer Start and End Actions")]

    [Tooltip("The action the timer will do when finished. This action takes Hit Data as a parameter")]
    [SerializeField] private frameAction timerFinishHitData;

    [Tooltip("The action the timer will do when finished.")]
    [SerializeField] private frameAction timerFinish;

    [Tooltip("The action the timer will do when it begins.")]
    [SerializeField] private frameAction timerStart;

    [System.NonSerialized] private myTimescale myTimeScale;

    [Space(2f)]
    [Header("Hit Data")]

    [Tooltip("The hit data to give to exit vibrate")]
    HitData hitData;

    public timer(frameAction timerStart, float time, frameAction timerFinish, HitData hitData, timerType name, myTimescale timeScale)
    {
        this.timerStart = timerStart;

        this.timeRemaining = time;

        if (timerFinish.getActionHitData() == null)
        {
            this.timerFinish = timerFinish;

            this.timerFinishHitData = null;
        }
        else
        {
            this.timerFinishHitData = timerFinish;

            this.timerFinish = null;
        }

        this.myTimeScale = timeScale;

        this.type = name;

        this.name = name.ToString();

        this.target = timer.timerType.Null;

        this.hitData = hitData;

        this.timeElapsed = 0f;

        executionActionsDisplay = new dictionaryDisplay();

    }

    //time and function to call back with name
    public timer(float time, frameAction timerFinish, timerType name, myTimescale timeScale)
    {
        this.timeRemaining = time;
        this.timerFinish = timerFinish;
        this.myTimeScale = timeScale;
        this.type = name;
        this.name = name.ToString();
        this.target = timer.timerType.Null;
        this.hitData = null;
        this.timeElapsed = 0f;
        this.timerFinishHitData = null;

        executionActionsDisplay = new dictionaryDisplay();

    }

    public timer(float time, frameAction timerFinish, timerType name, Dictionary<int, List<frameAction>> executionActions, myTimescale timeScale)
    {
        this.timeRemaining = time;
        this.timerFinish = timerFinish;
        this.myTimeScale = timeScale;
        this.type = name;
        this.name = name.ToString();
        this.target = timer.timerType.Null;
        this.hitData = null;
        this.executionActions = executionActions;
        this.timeElapsed = 0f;
        this.timerFinishHitData = null;

        executionActionsDisplay = new dictionaryDisplay();

        updateExecutionActionDictionaryDisplay();
    }

    //function to initially call, time, and function to call back with name
    public timer(frameAction timerStart, float time, frameAction timerCallBack, myTimescale timeScale, timerType name)
    {
        this.timerStart = timerStart;
        this.timeRemaining = time;
        this.timerFinish = timerCallBack;
        this.myTimeScale = timeScale;
        this.type = name;
        this.name = name.ToString();
        this.target = timer.timerType.Null;
        this.hitData = null;
        this.timeElapsed = 0f;
        this.timerFinishHitData = null;

        executionActionsDisplay = new dictionaryDisplay();
    }

    //target timer, function to initially call, time, and function to call back
    public timer(timer.timerType target, frameAction timerStart, float time, frameAction timerFinish, timerType name, myTimescale timeScale)
    {
        this.timerStart = timerStart;
        this.timeRemaining = time;
        this.timerFinish = timerFinish;
        this.myTimeScale = timeScale;
        this.type = name;

        this.name = name.ToString();
        this.target = target;
        this.hitData = null;
        this.timeElapsed = 0f;
        this.timerFinishHitData = null;

        executionActionsDisplay = new dictionaryDisplay();
    }

    public void clearExecutionActionDictionaryDisplay()
    {
        executionActionsDisplay.frames = null;

        executionActionsDisplay.actions = null;
    }

    public void updateExecutionActionDictionaryDisplay()
    {
        try
        {
            executionActionsDisplay.frames = new List<int>(executionActions.Keys);

            executionActionsDisplay.actions = new List<dictionaryDisplay.frameActionDisplay>();

            List<List<frameAction>> values = new List<List<frameAction>>(executionActions.Values);

            for(int i=0; i < values.Count; i++)
            {
                dictionaryDisplay.frameActionDisplay actionList = new dictionaryDisplay.frameActionDisplay();

                actionList.displayActions = new List<frameAction>(values[i]);

                executionActionsDisplay.actions.Add(actionList);
            }
        }
        catch
        {
            Debug.Log("error removing from execution actions Display dict");
        }

    }

    public void callActionOnFrame(int frame)
    {

        foreach (frameAction a in executionActions[frame])
        {
            a.getAction()();
        }
    }


    public void callTimerStart()
    {
        timerStart.getAction()();

        //if the timer has a target, add the time remaining to that timer's time remaining 
        if (target != timer.timerType.Null)
        {
            addToTimer(target, timeRemaining);
        }
    }

    public void callTimerFinish()
    {
        timerFinish.getAction()();

        this.setTime(0f);
    }

    public void callTimerFinishHitData(HitData hitData)
    {
        timerFinishHitData.getActionHitData()(hitData);

        this.setTime(0f);
    }

    public void removeTimerFromTimescale()
    {
        myTimeScale.removeTimer(this.type);

        myTimeScale.updateTimerDictionaryDisplay();
    }

    public float getTimeRemaining()
    {
        return this.timeRemaining;
    }
    public float getTimeElapsed()
    {
        return this.timeElapsed;
    }
    public string getName()
    {
        return this.name;
    }
    public HitData getHitData()
    {
        return this.hitData;
    }
    public timer.timerType getTarget()
    {
        return this.target;
    }
    public int getCurrentFrame()
    {
        return this.currentFrame;
    }

    public frameAction getTimerFinished()
    {
        return timerFinish;
    }

    public frameAction getTimerFinishedHitDat()
    {
        return timerFinishHitData;
    }

    public Dictionary<int, List<frameAction>> getExecutionActions()
    {
        return this.executionActions;
    }
    public void addActionsToExecute(int frame, List<frameAction> actions)
    {

        foreach (frameAction a in actions)
        {

            if (executionActions.ContainsKey(frame) && myTimeScale.getCharacter().timerInList(timer.timerType.attackTimer))
            {
                List<frameAction> tempActions = new List<frameAction>();

                tempActions.Add(a);

                if (myTimeScale.getCharacter().timerInList(timer.timerType.attackTimer))
                {

                    //make sure timer is present
                    try
                    {
                        executionActions.Add(frame, tempActions);

                        updateExecutionActionDictionaryDisplay();
                    }
                    //timer not present then update
                    catch
                    {
                        updateExecutionActionDictionaryDisplay();
                    }
                    //Do nothing
                    finally
                    {
                        
                    }
                    
                }
            }
            else if(myTimeScale.getCharacter().timerInList(timer.timerType.attackTimer))
            {
                //Attempt to add
                try
                {
                    executionActions[frame].Add(a);

                    updateExecutionActionDictionaryDisplay();
                }
                //add failed, update
                catch
                {
                    updateExecutionActionDictionaryDisplay();
                }
                //do nothing 
                finally
                {

                }
            }
        }
    }
    public void addActionsWithCurrentFrame(int frame, List<frameAction> actions)
    {
        addActionsToExecute((frame + this.getCurrentFrame()), actions);

        updateExecutionActionDictionaryDisplay();
    }

    public void setTime(float t)
    {
        timeRemaining = t;

        currentFrame = 0;

        timeElapsed = 0;

        try
        {
            myTimeScale.removeActionsToExecute(this.type); //XYZ
        }
        catch
        {

        }

        myTimeScale.updateTimerDictionaryDisplay();
    }

    public void addTime(float t)
    {
        timeRemaining += t;

        myTimeScale.updateTimerDictionaryDisplay();
    }

    public void subtractTime(float t)
    {
        timeRemaining -= t;

        timeElapsed += t;

        currentFrame++;

        myTimeScale.updateTimerDictionaryDisplay();
    }

    public void setTargetTimer(timer.timerType timerToSet, float t)
    {
        if (myTimeScale.getTimers().ContainsKey(timerToSet))
        {

            myTimeScale.getTimers()[timerToSet].setTime(t);

            myTimeScale.updateTimerDictionaryDisplay();

        }

        else Debug.Log(timerToSet + " is not in " + myTimeScale.getCharacter().name);
    }

    public void addToTimer(timer.timerType timerToAddTo, float t)
    {
        //Debug.Log("Adding "+t+ "to "+timerName);

        if (myTimeScale.getTimers().ContainsKey(timerToAddTo))
        {

            myTimeScale.getTimers()[timerToAddTo].addTime(t);

            Debug.Log(t+" NEW TIME IS: " + myTimeScale.getTimers()[timerToAddTo].getTimeRemaining());

            myTimeScale.updateTimerDictionaryDisplay();
        }

        else Debug.Log(timerToAddTo + " is not in " + myTimeScale.getCharacter().name);


    }

}

[System.Serializable]
public class myTimescale
{
    [Space(2f)]
    [Header("Delta and Physics Time")]

    [Tooltip("The bool which controls if the object should be frozen in time")]
    private bool shouldFreezeInTime;

    [Tooltip("Delta Time and fixed time for character for ")]
    public float myDeltaTime, myPhysicsTime;

    float storeDeltaTime, storePhysicsTime;

    [Tooltip("Character timers ")]
    private Dictionary<timer.timerType, timer> timers;

    [System.Serializable]
    public class dictionaryDisplay
    {
        public List<timer.timerType> timerNames;

        public List<timer> timers;
    }

    [Space(2f)]
    [Header("Timer Dictionary")]
    [Tooltip("Displays the Dictionary values")]
    [SerializeField] private dictionaryDisplay timerDictionaryDisplay;

    // Start is called before the first frame update

    private Character character;

    public myTimescale()
    {
        this.shouldFreezeInTime = false;

        this.myDeltaTime = 0f;

        this.myPhysicsTime = 0f;

        this.storeDeltaTime = 0f;

        this.storePhysicsTime = 0f;

        this.timers = new Dictionary<timer.timerType, timer>();

        this.timerDictionaryDisplay = new dictionaryDisplay();
    }

    public myTimescale(Character c)
    {
        this.character = c;

        this.shouldFreezeInTime = false;

        this.myDeltaTime = 0f;

        this.myPhysicsTime = 0f;

        this.storeDeltaTime = 0f;

        this.storePhysicsTime = 0f;

        this.timers = new Dictionary<timer.timerType, timer>();

        this.timerDictionaryDisplay = new dictionaryDisplay();
    }

    public void updateTimerDictionaryDisplay()
    {
        timerDictionaryDisplay.timerNames = new List<timer.timerType>(timers.Keys);

        timerDictionaryDisplay.timers = new List<timer>(timers.Values);
    }


    public bool timerInList(timer.timerType name)
    {

        if (timers.ContainsKey(name)) return true;

        return false;

    }

    public void addToTimerInList(timer.timerType name, float time)
    {

        if (timers.ContainsKey(name))
        {
            timers[name].addTime(time);
        }
    }

    public void setTimerInList(timer.timerType name, float time)
    {
        if (timers.ContainsKey(name))
        {
            timers[name].setTime(time);
        }

    }

    public void addCurrentActionsToExecute(timer.timerType name, Dictionary<int, List<frameAction>> actions)
    {
        if (timers.ContainsKey(name))
        {
            foreach (var a in actions)
            {
                timers[name].addActionsWithCurrentFrame(a.Key, a.Value);
            }
        }
    }

    public void addActionsToExecute(timer.timerType name, Dictionary<int, List<frameAction>> actions)
    {
        if (timers.ContainsKey(name))
        {
            foreach (var a in actions)
            {
                timers[name].addActionsToExecute(a.Key, a.Value);
            }
        }
    }

    public void removeActionsToExecute(timer.timerType name)
    {

        if (timers.ContainsKey(name))
        {
            List<int> timerFrames = new List<int>(timers[name].getExecutionActions().Keys);

            for(int i=0; i< timerFrames.Count; i++)
            {
                timers[name].getExecutionActions()[i] = null;
            }

            try
            {
                //Debug.Log("ERR 1");

                timers[name].clearExecutionActionDictionaryDisplay();

                //Debug.Log("ERR 2");

                timers[name].getExecutionActions();

                //Debug.Log("ERR 3");

                timers[name].getExecutionActions().Clear();

            }
            catch
            {
                Debug.Log("Cannot clear removeActionsToExecute");
            }
        }
    }

    public Dictionary<timer.timerType, timer> getTimers()
    {
        return timers;
    }

    public timer getTimer(timer.timerType name)
    {
        if (timers.ContainsKey(name)) return timers[name];

        else Debug.Log(name + "is not in " + getCharacter().name);

        return null;
    }

    public Character getCharacter()
    {
        return character;
    }

    //time func call back, and name
    public void createTimer(float time, frameAction timerFinish, timer.timerType name)
    {
        timer t = new timer(time, timerFinish, name,this);

        timers.Add(name, t);

        updateTimerDictionaryDisplay();
    }

    public void createTimer(float time, frameAction timerFinish, timer.timerType name, Dictionary<int, List<frameAction>> executionActions)
    {
        timer t = new timer(time, timerFinish, name, executionActions, this);

        timers.Add(name, t);

        updateTimerDictionaryDisplay();
    }

    public void createTimer(frameAction timerCall, float time, frameAction timerFinish, HitData hitData, timer.timerType name)
    {
        timer t = new timer(timerCall, time, timerFinish, hitData, name, this);

        timers.Add(name, t);

        t.callTimerStart();

        updateTimerDictionaryDisplay();
    }

    //func to initially call, time, funct to call back, and name
    public void createTimer(frameAction timerStart, float time, frameAction timerFinish, timer.timerType name)
    {
        timer t = new timer(timerStart, time, timerFinish, this, name);

        timers.Add(name, t);

        t.callTimerStart();

        updateTimerDictionaryDisplay();
    }

    //target, func to initially call, time, funct to call back
    public void createTimer(timer.timerType target, frameAction timerStart, float time, frameAction timerFinish, timer.timerType name)
    {
        timer t = new timer(target, timerStart, time, timerFinish, name, this);

        timers.Add(name, t);

        t.callTimerStart();

        updateTimerDictionaryDisplay();
    }

    // Update is called once per frame
    public void tickUpdate()
    {
        //if should freeze in time, set time to zero and continue to store time
        if (shouldFreezeInTime)
        {
            //myDeltaTime is equal to the current time minus the time on the rpevious frame
            myDeltaTime = 0f;
            //store the time on the previous frame
            storeDeltaTime = Time.time;
            //Debug.Log("MY: " + myDeltaTime + " VS. DELTA: " + Time.deltaTime);

            return;
        }
        //myDeltaTime is equal to the current time minus the time on the rpevious frame
        myDeltaTime = Time.time - storeDeltaTime;
        //store the time on the previous frame
        storeDeltaTime = Time.time;
        //Debug.Log("MY: " + myDeltaTime + " VS. DELTA: " + Time.deltaTime);
    }



    public void tickFixedUpdate()
    {
        //Remove Finished Timers

        List<timer.timerType> keyList = new List<timer.timerType>(timers.Keys);
        foreach (timer.timerType timerName in keyList)
        {
            if (timers[timerName].getTimeRemaining() <= 0f)
            {
                timers[timerName].removeTimerFromTimescale();

                continue;
            }

            if (timers[timerName].getTimeRemaining() > 0f)
            {
                timers[timerName].subtractTime(Time.fixedDeltaTime);

                checkExecuteAction(timers[timerName]);

                if (timers[timerName].getTimeRemaining() <= 0f)
                {
                    if (timers[timerName].getTimerFinished() != null)
                    {
                        if (timers[timerName].getTimerFinished().getName()!=null) {
                            timers[timerName].callTimerFinish();
                        }
                    }
                    if (timers[timerName].getTimerFinishedHitDat() != null)
                    {
                        if (timers[timerName].getTimerFinishedHitDat().getName() != null)
                        {
                            timers[timerName].callTimerFinishHitData(timers[timerName].getHitData());
                        }
                    }
                }
            }
        }

        //if should freeze in time, set time to zero and continue to store time
        if (shouldFreezeInTime)
        {
            //myDeltaTime is equal to the current time minus the time on the rpevious frame
            myPhysicsTime = 0f;
            //store the time on the previous frame
            storePhysicsTime = Time.time;
            //Debug.Log("MY: " + myPhysicsTime + " VS. PHYS: " + Time.fixedDeltaTime);

            return;
        }
        //myDeltaTime is equal to the current time minus the time on the rpevious frame
        myPhysicsTime = Time.time - storePhysicsTime;
        //store the time on the previous frame
        storePhysicsTime = Time.time;
        //Debug.Log("MY: " + myPhysicsTime + " VS. PHYS: " + Time.fixedDeltaTime);

        return;
    }

    void checkExecuteAction(timer t)
    {
        if (t.getExecutionActions() == null)
        {
            //Debug.Log("NONE");
            return;
        }

        foreach (int frame in t.getExecutionActions().Keys)
        {
            if (frame == t.getCurrentFrame())
            {
                Debug.Log("MY CURRENT: " + frame + " " + t.getCurrentFrame());

                t.callActionOnFrame(frame);
            }
        }
    }


    public bool isTimerComplete(float timerToCheck)
    {
        return timerToCheck <= 0f;
    }

    public void removeTimer(timer.timerType timerToRemove)
    {
        if (timers.ContainsKey(timerToRemove))
        {
            timers[timerToRemove] = null;

            timers.Remove(timerToRemove);

            updateTimerDictionaryDisplay();
        }

    }



}

