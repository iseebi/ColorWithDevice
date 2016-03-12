# ColorWithDevice - IoT + iOS/Android アプリ + クラウドまで全部 C# で書いてしまうデモ

以下のイベントで使用したデモです。

- [Developers Summit 2016 Japan](http://event.shoeisha.jp/devsumi/20160218/) Xamarin User Group コミュニティブース
- [第3回フェンリルデベロッパーズセミナー](https://fenrir.doorkeeper.jp/events/37782) セッション「モバイル/クラウド/IoT…C# の新たな活躍の場」

このような動きをします。

- Gadgeteer と iOS/Android アプリを Bluetooth で接続します。
- ジョイスティックの向きに合わせて LED の色が変わります
- その色を Bluetooth Low Energy で接続された iOS/Android に送られ、画面上に同じ色が表示されます。
- Gadgetter につながっていないアプリにも、SignalR を通じて同じ色が表示されます。

## .NET Gadgeteer の接続

以下のモジュールを使います。

- メインボード: FEZ Cerberus 1.1
- Socket 4: Joystick 1.2 (Socket A)
- Socket 6: Bluetooth SMART (Socket U)
- Socket 7: LED7C 1.0 (Socket X)

## サーバー側のデプロイ

[Installing ASP.NET 5 On Linux](http://docs.asp.net/en/latest/getting-started/installing-on-linux.html) に従ってセットアップした Ubuntu で動きます。

- ColorWithDevice.Model を先に xbuild でビルドしておく必要があります。
- DNVM では mono を指定してください。

実行する際は `dnx web` で実行するとローカルのみ、`dnx public` で実行すると外部からの接続可で起動します。

## アプリのビルド

サーバーを動かしている URL を ColorWithDevice の App.cs で指定してください。

## License

MIT