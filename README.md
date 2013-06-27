Unity-SmartArrayPropertyDrawer
==============================

##概要
Unityの配列に「途中挿入」「途中削除」の機能を追加するPropertyDrawer。  
ユーザ定義クラスの配列にも「だいたい」対応する。

現時点での問題点は「既知の問題」を参照。

##書き方

    [SmartArray]
    int [] intArray;
    
    [SmartArray]
    List<Vector2> v2List;

    [SmartArray]
    MyClass [] myclassArray;

こんなかんじ。

##使い方
見た目はデフォルトの配列表記と同じ。

###Sizeフィールド
Sizeフィールド右クリックすると以下のメニューが表示される。
* Add/First  
  配列の始端に新要素を追加。
* Add/Last  
  配列の終端に新要素を追加。
* Remove/First  
  配列の始端を削除。
* Remove/Last  
  配列の終端を削除。

###Elementフィールド
Elementフィールド右クリックすると以下のメニューが表示される。ここでXは右クリックで選んだ配列要素のインデックスになる。
* Add/Begore 'Element X'  
  X番目の配列要素の直前に新要素を追加。
* Add/After 'Element X'  
  X番目の配列要素の直後に新要素を追加。
* Remove/'Element X'
  X番目の配列要素を削除。

##既知の問題

###軽度 - 少し不便

1. UnityEngine.Object 系配列の途中削除コマンドは、対象がNULL以外のときNULL代入となり、対象がNULLのとき配列要素削除になる。  
  -> Object系のみ特殊処理を追加予定。

2. 既存のPropertyDrawerと併用はできない。たとえば'[SmartArray] [Range(0,1)] float [] floatArray;'はできない。ユーザ定義クラス内では「重度-1,2」に抵触しない限り可能。

###中度 - 操作性に影響発生

1. 巨大配列（Inspector描画範囲より長くなる配列）の描画が、Inspectorデフォルトのものより重い。  
  -> 描画不要な範囲の描画を省略するアルゴリズムの追加予定。特定の変数型では描画要素の高さが不変なのでキャッシュを用いて高速化予定。


###重度 - 致命的に操作性が低下

1. 次のUnityEngine系class配列は描画できない。 Bounds, Gradient, AnimationCurve. これはUnity自体のバグであり、デフォルトで描画されずPropertyDrawer自体が起動しないため対処不能と判断し、現時点で放置している。

2. ユーザ定義クラスが、配列描画を変更するPropertyDrawer（SmartArray自身を含む）を使用している場合、致命的に描画が崩壊する。

3. Windowsのみでテスト。Macでどう動くか知らない。

##更新履歴

1. 2013/06/27  初回コミット。
