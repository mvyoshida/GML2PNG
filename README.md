# GML2PNG
国土地理院のGMLファイルからPNGを出力するツール。

データがない部分は0、それ以外は標高に10下駄をはかせています。

データの中の最高標高部分が白になるようにしています。

使い方

ビルドして起動するとウィンドウが出てきます。

国土地理院から「基盤地図情報　数値標高モデル」のデータをダウンロードしてきます。
展開するとxmlファイルがありますので、それをウィンドウにドラッグ&ドロップします。

必要なファイルをすべてドラッグ&ドロップで放り込んだら「Export」ボタンを押します。
ファイルの保存先を聞かれますので任意の場所に保存してください。

ファイル名はデフォルトで
「heightMap({longitudeMin},{latitudeMin})-({longitudeMax},{latitudeMax})[{elevationMin},{elevationMax}].png」
となっています。
それぞれ意味は

{longitudeMin}最小経度
{latitudeMin}最小緯度
{longitudeMax}最大経度
{latitudeMax}最大緯度
{elevationMin}標高の最低値
{elevationMax}標高の最大値+10

です。


