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
    IMapUnit[,] Children { get; }


    List<MapUnitPair> SelfLeftEntries { get; }
    List<MapUnitPair> SelfRightEntries { get; }
    List<MapUnitPair> SelfTopEntries { get; }
    List<MapUnitPair> SelfBottomEntries { get; }

   
    IMapUnit GetChildCluster(Point position);
    IMapUnit GetChildCluster(int line, int col);



    void ComputePathTable();
    void LinkClustersByEntries();
    List<MapUnitPair> GetSharedBorder(ICluster other);
    void Write(BinaryWriter bw);
    void Read(BinaryReader br);
   



}
