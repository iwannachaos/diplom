using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public interface ICluster:IMapUnit
{
    int Width { get; }
    int Height { get; }
    int ChildrenWidth { get; }
    int ChildrenHeigth { get; }


    List<IMapUnit> SelfLeftEntries { get; }
    List<IMapUnit> SelfRightEntries { get; }
    List<IMapUnit> SelfTopEntries { get; }
    List<IMapUnit> SelfBottomEntries { get; }

   
    IMapUnit GetChildCluster(Point position);
    IMapUnit GetChildCluster(int line, int col);



    void ComputePathTable();
    void LinkClustersByEntries();
    void Write(BinaryWriter bw);
    void Read(BinaryReader br);
   



}
