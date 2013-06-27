Unity-SmartArrayPropertyDrawer
==============================

Unityの配列に「途中挿入」「途中削除」の機能を追加するPropertyDrawer。
ユーザ定義クラスの配列にも「だいたい」対応する。
現時点での問題点は「既知の問題３，４」を参照。

----- 使い方 -----

[SmartArray]
int [] intArray;

[SmartArray]
List<Vector2> v2List;

[SmartArray]
MyClass [] myclassArray;

こんなの。

> v2List
    Size        2      <- ここを右クリックすることで、配列先頭/後端に要素を追加/削除するメニューが開く。
  > Element 1
  > Element 2          <- ここを右クリックすることで、Element 2 の前方/後方に追加、またはElement 2を削除できる。他のPropertyDrawerとの兼ね合い上、Element折り畳み部分のみメニュー判定がある。
      x         0
      y         0

----- 既知の問題 -----

[軽度]
1-1. [軽度] UnityEngine.Object 系配列の途中削除コマンドは、対象がNULL以外のときNULL代入となり、対象がNULLのとき配列要素削除になる。
  -> Object系のみ特殊処理を追加予定。

[中度]
2-1. [軽度] 巨大配列（Inspector描画範囲より長くなる配列）の描画が、Inspectorデフォルトのものより重い。
  -> 描画不要な範囲の描画を省略するアルゴリズムの追加予定。特定の変数型では描画要素の高さが不変なのでキャッシュを用いて高速化予定。
2-2. [中度] 既存のPropertyDrawerと併用はできない。たとえば[SmartArray] [Range(0,1)] float [] floatArray;はできない。なんか方法ないだろうか？

[重度]
3-1. [重度] 次のUnityEngine系class配列は描画できない。 Bounds, Gradient, AnimationCurve. これはUnity自体のバグであり、デフォルトで描画されずPropertyDrawer自体が起動しないため対処不能と判断し、現時点で放置している。
3-2. [重度] ユーザ定義クラスが、配列描画を変更するPropertyDrawer（SmartArray自身を含む）を使用している場合、致命的に描画が崩壊する。


----- 更新履歴 -----

2013/06/27  初回コミット。