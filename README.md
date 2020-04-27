# Virtual Office

![image](https://user-images.githubusercontent.com/1011232/79879948-52bf8f00-842a-11ea-9b5b-53676965855a.png)

## Motivation

快適なWork From Homeを模索中の現在、ほしいと思ったので作り始めました。

Slackのコールは一息付きたくて同僚にかけるには敷居が高い。巷ではDiscord出社がはやっている用だが、Slackをメインツールとして使っているため機能がかぶり過ぎる感じがする。調べた限りではremoや[Online Twon](https://hn.town.siempre.io/)がフィットしそうなので、似たようなものを目指します。

これはマネージャーが部下の仕事ぶりを監視するためのものではありません。🙅‍♂️

## Dependencies

+ WebRTC部分はとりあえずSkyWayでサクッと実装します。
+ ユーザ情報の共有はSignalRで実装します。

## How to use

Azure WebAppsなどにデプロイ。

### 画面共有のお絵描き機能

画面共有を見てる側：画面共有の動画上をマウスでぐりぐりすると手書きが可能です。書いた内容は共有されます。
画面共有をしてる側：他の参加者によってかかれたないようをスクリーン上に投影するには別途デスクトップアプリ（[screen drawer](https://github.com/iwate/screen-drawer/)）を立ち上げる必要があります。

## LICENSE

MIT
