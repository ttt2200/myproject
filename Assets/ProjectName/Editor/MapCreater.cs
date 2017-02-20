using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class MapCreater : EditorWindow {

	// 画像ディレクトリ
	private Object imgDirectory;
	// 出力先ディレクトリ(nullだとAssets下に出力します)
	private Object outputDirectory;
	// マップエディタのマスの数
	private int mapSize = 10;
	// グリッドの大きさ、小さいほど細かくなる
	private float gridSize = 50.0f;
	// 出力ファイル名
	private string outputFileName;
	// 選択した画像パス
	private string selectedImagePath;

	[UnityEditor.MenuItem("Window/MapCreater")]
	static void Open()
	{
		// メニューのWindow/EditorExを選択するとOpen()が呼ばれる。
		// 表示させたいウィンドウは基本的にGetWindow()で表示＆取得する。
		EditorWindow.GetWindow<MapCreater>("MapCreater"); // タイトル名を"EditorEx"に指定（後からでも変えられるけど）
	}

	// Windowのクライアント領域のGUI処理を記述

	void OnGUI()
	{
		// GUI
		GUILayout.BeginHorizontal();
		GUILayout.Label("Image Directory : ", GUILayout.Width(110));
		imgDirectory = EditorGUILayout.ObjectField(imgDirectory, typeof(UnityEngine.Object), true);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("map size : ", GUILayout.Width(110));
		mapSize = EditorGUILayout.IntField(mapSize);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("grid size : ", GUILayout.Width(110));
		gridSize = EditorGUILayout.FloatField(gridSize);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Save Directory : ", GUILayout.Width(110));
		outputDirectory = EditorGUILayout.ObjectField(outputDirectory, typeof(UnityEngine.Object), true);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Save filename : ", GUILayout.Width(110));
		outputFileName = (string)EditorGUILayout.TextField(outputFileName);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		DrawImageParts();
	}

	// 画像一覧をボタン選択出来る形にして出力
	private void DrawImageParts()
	{
		if (imgDirectory != null)
		{
			float x = 0.0f;
			float y = 00.0f;
			float w = 50.0f;
			float h = 50.0f;
			float maxW = 300.0f;

			string path = AssetDatabase.GetAssetPath(imgDirectory);
			string[] names = Directory.GetFiles(path, "*.png");
			EditorGUILayout.BeginVertical();
			foreach (string d in names)
			{
				if (x > maxW)
				{
					x = 0.0f;
					y += h;
					EditorGUILayout.EndHorizontal();
				}
				if (x == 0.0f)
				{
					EditorGUILayout.BeginHorizontal();
				}
				GUILayout.FlexibleSpace();
				Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(d, typeof(Texture2D));
				if (GUILayout.Button(tex, GUILayout.MaxWidth(w), GUILayout.MaxHeight(h), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
				{
					selectedImagePath = d;
				}
				GUILayout.FlexibleSpace();
				x += w;
			}
			EditorGUILayout.EndVertical();
		}
	}
}
