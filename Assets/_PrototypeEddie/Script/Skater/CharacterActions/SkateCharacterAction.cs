using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class for Advance Actions
/// </summary>
public class SkateCharacterAction : MonoBehaviour
{
    protected Skater _ParentCharacter;
    protected bool _IsExecuting = false;
    public bool IsExecuting => _IsExecuting;

    /// <summary>
    ///  When Action is complete.
    ///  Passes Bool if Action was Success.
    /// </summary>
    public Action<bool> OnActionComplete = new Action<bool>((success) => {});
    protected void CharacterActionInit()
    {
        _ParentCharacter = GetComponentInParent<Skater>();
    }

    public virtual void StartAction()
    {
        _IsExecuting = true;
    }

    /// <summary>
    /// Complete/Stop this action if running.
    /// </summary>
    /// <param name="Successful">Was this task successful?</param>
    /// <returns>Returns false if the action was not running.</returns>
    protected virtual bool FinishAction(bool Successful)
    {
        if (_IsExecuting)
        {
            _IsExecuting = false;
            OnActionComplete.Invoke(Successful);
            return true;
        }

        return false;

    }
    /// <summary>
    /// Start : Init Method for Character Action.
    /// <para>Will bind itself to SkateCharacter.
    /// Other children of this class must call this method
    /// in their Start Method</para>
    /// </summary>
    protected void Start()
    {
        CharacterActionInit();
    }
    void FixedUpdate()
    {
    }

}
