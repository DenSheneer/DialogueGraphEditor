using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;

public class SceneDialogue : SceneEvent
{
    [SerializeField]
    private DialogueContainer dialogueObject;
    private Dictionary<string, DialogueNodeData> nodeCache;

    private GameObject UI, choicePanel;
    private TextMeshProUGUI buttonText_a, buttonText_b, actorNameContainer, dialogueTextContainer;
    private Button button_a, button_b;


    bool waitingForInput = false;
    DialogueNodeData currentNode = null;
    DialogueNodeData nextNode = null;

    private void Awake()
    {
        findAndVerifyComponents();
        resetChoicePanel();
    }
    public void Start()
    {
        if (dialogueObject.DialogueNodeData.Count > 0)
        {
            generateCache();
            loadDialogue(findStartNode());
        }

    }

    private void generateCache()
    {
        nodeCache = new Dictionary<string, DialogueNodeData>();
        foreach (var node in dialogueObject.DialogueNodeData)
        {
            nodeCache.Add(node.Guid, node);
        }
    }
    public override void Progress()
    {
        if (waitingForInput)
            return;

        if (nextNode != null)
        {
            loadDialogue(nextNode);
        }
        else
        {
            onEnd?.Invoke();
        }
    }

    private void loadDialogue(DialogueNodeData node)
    {
        if (node != null)
        {
            currentNode = node;
            showNode(currentNode);

            var next = findNextNode();
            if (next.Count > 0)
            {
                if (next.Count == 1)
                {
                    nextNode = next[0];
                }
                else
                {
                    waitingForInput = true;
                    showChoices(next);
                }
            }
            else
            {
                nextNode = null;
                Debug.Log("End of the graph reached.");
            }
        }
    }
    private List<DialogueNodeData> findNextNode()
    {
        var connections = dialogueObject.NodeLinks.Where(x => x.BaseNodeGuid == currentNode.Guid).ToList();
        List<DialogueNodeData> nextNodes = new List<DialogueNodeData>();

        if (connections.Count > 0)
        {
            foreach (var connection in connections)
            {
                if (nodeCache.ContainsKey(connection.TargetNodeGuid))
                {
                    nextNodes.Add(nodeCache[connection.TargetNodeGuid]);
                }
            }
        }
        return nextNodes;
    }
    virtual protected void showChoices(List<DialogueNodeData> options)
    {
        if (options.Count > 1)
        {
            choicePanel.SetActive(true);
            if (options.Count > 2)
            {
                Debug.Log("Warning! This dialogue graph had >2 options for this branch.\n " +
                    "       This demo was meant to have a max of 2. The remaining options will not be displayed.");
            }

            buttonText_a.text = options[0].DialogueText;
            buttonText_b.text = options[1].DialogueText;

            button_a.onClick.AddListener(() => loadDialogue(options[0]));
            button_a.onClick.AddListener(() => resetChoicePanel());

            button_b.onClick.AddListener(() => loadDialogue(options[1]));
            button_b.onClick.AddListener(() => resetChoicePanel());
        }
        else
            Debug.Log("Warning! Trying to display 1 choice as a multiple choice branch");
    }
    void resetChoicePanel()
    {
        waitingForInput = false;
        choicePanel.SetActive(false);
        button_a.onClick.RemoveAllListeners();
        button_b.onClick.RemoveAllListeners();
    }
    virtual protected void showNode(DialogueNodeData node)
    {
        if (node != null)
        {
            if (node.Actor != null)
                if (node.Actor == null || node.Actor.name == "_None")
                    actorNameContainer.text = string.Empty;
                else
                    actorNameContainer.text = node.Actor.name;

            dialogueTextContainer.text = node.DialogueText;
        }
    }
    private DialogueNodeData findStartNode()
    {
        foreach (var tmp in dialogueObject.NodeLinks)
        {
            if (tmp.PortName == "Next")
            {
                return nodeCache[tmp.TargetNodeGuid];
            }
        }

        return null;
    }
    /// <summary>
    /// This function finds and null-checks all required components for safe usage. <br></br>
    /// Call in Awake().
    /// </summary>
    protected virtual void findAndVerifyComponents()
    {
        _ = dialogueObject ?? throw new ArgumentException("DialogueObject not found");

        UI = GameObject.Find("UI");
        _ = UI ?? throw new ArgumentNullException("UI object not found");

        var dialogueTextObject = UI.gameObject.transform.Find("DialogueTextObject");
        _ = dialogueTextObject ?? throw new ArgumentNullException("DialogueTextObject not found");

        dialogueTextContainer = dialogueTextObject.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _ = dialogueTextContainer ?? throw new ArgumentNullException("DialogueTextContainer not found");

        var actorNameObject = UI.gameObject.transform.Find("ActorName");
        _ = actorNameObject ?? throw new ArgumentNullException("ActorNameObject not found");

        actorNameContainer = actorNameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _ = actorNameContainer ?? throw new ArgumentNullException("ActorNameContainer not found");

        choicePanel = GameObject.Find("ChoicePanel");
        _ = choicePanel ?? throw new ArgumentNullException("ChoicePanel not found");

        var buttons = choicePanel.GetComponentsInChildren<Button>();
        if (buttons.Length > 1)
        {
            button_a = buttons[0];
            _ = button_a ?? throw new ArgumentNullException("Button_a not found");

            button_b = buttons[1];
            _ = button_b ?? throw new ArgumentNullException("Button_b not found");
        }
        else { throw new ArgumentNullException("Did not find enough button objects"); }


        buttonText_a = button_a.GetComponentInChildren<TextMeshProUGUI>();
        _ = buttonText_a ?? throw new ArgumentNullException("ButtonText_a not found");

        buttonText_b = button_b.GetComponentInChildren<TextMeshProUGUI>();
        _ = buttonText_b ?? throw new ArgumentNullException("ButtonText_b not found");
    }
}
