# YUTOROOM Essentials

日本の風呂ワールドに不可欠なギミック群

## インストール

### VCCによる方法

1. https://vpm.narazaka.net/ から「Add to VCC」ボタンを押してリポジトリをVCCにインストールします。
2. VCCでSettings→Packages→Installed Repositoriesの一覧中で「Narazaka VPM Listing」にチェックが付いていることを確認します。
3. アバタープロジェクトの「Manage Project」から「YUTOROOM Essentials」をインストールします。

## 使い方

### BathBombSystem

入浴剤によるお湯の色変化

![inspector](docs~/BathBombSystem.png)

- VRCPickup+ObjectSyncで投げられる入浴剤コライダーと、同数のマテリアル、アクティブオブジェクトを指定します。
- アクティブオブジェクトには水面下PostProcessing Volumeなどを指定します。

### DrinkSound

ドリンクを飲む音

#### セットアップ

1. VRCPickupのついた飲み物オブジェクトに`DrinkSoundPickup`を付けます。
2. その飲み物オブジェクトの子に`DrinkSound` prefabを配置します。
3. それぞれを参照させます。
4. `DrinkSound`のAudioSourceに飲む音を設定します。（飲む音を設定したprefab variantを作ると便利だと思います。）
  - YUTOROOMの飲む音はこちらを使用しました。  [ごくごく飲む（ニコニ・コモンズ）](https://commons.nicovideo.jp/works/nc44239)

#### 既知の問題

- 同期されて聞こえるようにしようかと思ったがやめた。
  - その名残でDrinkSoundPickupがManual Syncになっているため、飲み物オブジェクトには[ManualObjectSync](https://github.com/mimyquality/FukuroUdon/wiki/Manual-ObjectSync)を付けることを推奨します。

### FloatingObject

風呂の水面に浮くオブジェクト

- [風呂桶](https://misagon339.booth.pm/items/2002692)のような凹面があり空気が入るオブジェクト
- [アヒルのおもちゃ（ラバーダック）](https://okpshop.booth.pm/items/2214230)のような上面が決定しているオブジェクト
- [浮き輪](https://tinmeshi.booth.pm/items/4938288)のような上下面が反転可能なオブジェクト

などを自然に浮かせる事が出来ます。

上手い具合に数値調整などをする必要があり設定はそれなりに煩雑です。

#### (共通) 水の領域コライダーを設定する

- 水の領域コライダー`WaterCollider`
- 水面高さTransform`WaterColliderTop`
- 水面直上の大気領域コライダー`AirCollider`

を設定します。

![colliders](docs~/FloatingObject01.png)

#### 風呂桶（凹面があり空気が入るオブジェクト）のセットアップ

サンプルは`Samples/FloatingObject/風呂桶sample.prefab`にあります。

VRCPickupのついた風呂桶オブジェクトの下に以下を設定します。

- 開口部を覆うコライダーを作り、`FloatingObjectTop`コンポーネントを追加します。
  - 下記の`FloatingObject`コンポーネントを参照させます。
- オブジェクトの形状に沿ったコライダーを作り、`FloatingObject`コンポーネントを追加します。
- サンプルを参考に各種設定を行ってください。
  - CanInvertはfalse

![inspector](docs~/FloatingObject_oke.png)

#### アヒルのおもちゃ（上面が決定しているオブジェクト）

サンプルは`Samples/FloatingObject/ラバーダックsample.prefab`にあります。

- オブジェクトの形状に沿ったコライダーを作り、`FloatingObject`コンポーネントを追加します。
- サンプルを参考に各種設定を行ってください。

![inspector](docs~/FloatingObject_rubberduck.png)

#### 浮き輪（上下面が反転可能なオブジェクト）

サンプルは`Samples/FloatingObject/裏表sample.prefab`にあります。

- オブジェクトの形状に沿ったコライダーを作り、`FloatingObject`コンポーネントを追加します。
- サンプルを参考に各種設定を行ってください。

![inspector](docs~/FloatingObject_yuzu.png)

## 更新履歴

- 1.0.0
  - リリース

## License

[Zlib License](LICENSE.txt)
