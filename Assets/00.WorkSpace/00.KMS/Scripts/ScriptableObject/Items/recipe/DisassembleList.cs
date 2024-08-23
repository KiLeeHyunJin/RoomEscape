using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisassembleList", menuName = "DisassembleList")]
public class DisassembleList : ScriptableObject // 분해리스트 스크립터블 오브젝트
{
    public Disassemble[] disassembles;
}

[Serializable]
public class Disassemble
{
    public string name;
    public string description;
    public ScriptableItem item;
    public ScriptableItem[] resolvents;
}