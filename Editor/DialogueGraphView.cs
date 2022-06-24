using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(x: 150, y: 200);
    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }

    private Port GeneratePort(DialogueNode target, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return target.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));
        return node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    public DialogueNode CreateDialogueNode(string nodeName, DialogueActor actor = null)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };
        if (actor != null) { dialogueNode.Actor = actor; }

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        var button = new Button(() => { AddChoiceport(dialogueNode); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.DialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);

        var actorObjects = Resources.LoadAll("Actors/", typeof(DialogueActor)).Cast<DialogueActor>().ToList();
        var listElements = new Dictionary<int, VisualElement>();

        var actorListView = new ListView();
        dialogueNode.inputContainer.Add(actorListView);
        actorListView.makeItem = () => new Label();
        actorListView.bindItem = (item, index) =>
        {
            DialogueActor thisActor = actorObjects[index];

            (item as Label).text = thisActor.name;
            listElements.Add(thisActor.GetHashCode(), item);

            if (dialogueNode.Actor != null && dialogueNode.Actor.GetHashCode() == thisActor.GetHashCode())
            {
                highlightListElement(item, thisActor.HighlightColor);
            }
            if (thisActor.name == "_None")
            {
                (item as Label).text = "None";
                if (actor == null)
                {
                    dialogueNode.Actor = thisActor;
                    highlightListElement(item, thisActor.HighlightColor);
                }
            }
        };
        actorListView.itemsSource = actorObjects;

        actorListView.onSelectionChange += (selectedObjects) =>
        {
            DialogueActor thisActor = selectedObjects.First() as DialogueActor;
            dialogueNode.Actor = thisActor;
            clearListHighlights(listElements.Values.ToList());
            highlightListElement(listElements[thisActor.GetHashCode()], thisActor.HighlightColor);
        };

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector3.zero, DefaultNodeSize));

        return dialogueNode;
    }

    public void AddChoiceport(DialogueNode dialogueNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Choice {outputPortCount}"
            : overriddenPortName;


        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "Remove"
        };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName &&
            x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    void clearListHighlights(List<VisualElement> visualElements)
    {
        foreach (var element in visualElements)
        {
            element.style.backgroundColor = Color.clear;
        }
    }
    void highlightListElement(VisualElement visualElement, Color highLightColor)
    {
        highLightColor.a = 0.25f;
        visualElement.style.backgroundColor = highLightColor;
    }
}
