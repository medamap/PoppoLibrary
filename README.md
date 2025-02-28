# ぽっぽライブラリ

## 概要
- Unity でドメイン駆動オニオンアーキテクチャスタイルで開発するためのライブラリです

## 要約
- なるべくUnity固有の機能に依存したプログラムを書かない
- アプリケーションの役割の種類毎に分離し、ごっちゃに行き来するアプリは蜜結合となり、後になって技術負債で苦しむ事になります
- ドメイン駆動とは、そのような苦しみを避けるための設計手法で、後になっても変更がしやすい設計を目指します
- 思い付きでクラスを作り、プロパティを追加し、SerializeField まみれのなんでも出来るスーパークラスは作ってはいけません
- 開発初期は面白いように進捗が進みますが、後になってからの修正が困難になります

## じゃぁ、具体的にどうしたらいいの？
- ネットワーク、データの保存、オブジェクトの操作、オブジェクトの描画、データの加工、アプリ固有の処理など、カテゴリ毎に明確に責任を分割し、ドメインとして定義します
- 各々のドメインは、お互い呼びあう事がないようにすることで、蜜結合を防ぎ、後になって苦しむ事が減ります
- ドメインの中もいくつかのレイヤーに分割し、それぞれのレイヤーが明確な責任を持つようにします
  - Presentation 層 ... MonoBehaviour を敬称したり、Unity 固有の機能に密接に結合が赦されます、ここから直接アクセスが赦されるのは Application 層までです
  - Application 層 ... ドメインの中心にアクセスが許される層で、ここから Infrastructure 層または Domain 層へのアクセスが許されますが、基本的には Infrastructure 層へのアクセスが許されます
  - Infrastructure 層 ... データの保存、ネットワーク、オブジェクトの操作など、具体的にやりたい事をここに記述します
  - Domain 層 ... 取り扱うデータの定義や Infrastructure 層で行う処理をインターフェイスとして定義を記述します

## このライブラリは
- このライブラリは、上記の考え方に基づいて実装されており、また、その実装をサポートするための機能を提供します

## ここで Unity の苦しんだ記憶を書いてみます
- Unity で MonoBehaviour のシングルトンを覚えて狂喜乱舞した記憶はあるでしょう
- これで明確に定義されたサービスに容易にアクセスでき、便利になったと使いまくった事は誰でもあるでしょう
- しかし、気が付けば SerializeField まみれ、シングルトンどうしに依存しあい、気が付けばどこか修正するとどこかに悪影響が、そしてまた増える SerializeField とシングルトン、もはやあなたはシングルトン地獄から逃れられない
- そしてやってくる応援の人員、彼は地獄のように依存の応酬のシングルトン天国に気が狂い、そして無駄に費やされる保守コスト、もはや誰も幸せになれない、そんな地獄、あなたはそんな開発環境にしたいのでしょうか
- 「これ、壊してゼロから作り直そうぜ」誰かの一言に「いや、もうここに実績のあるシステムがあるんだ、お前責任とれるのか？」そして再び全員が地獄に飲み込まれていくのでした

## なんでも出来るスーパークラスは肥大化した貧乏神のようである
- あたかも熟練者が作ったかのような SerializeField まみれのスーパークラス、それは初期設計コストをケチったツケで「技術負債」と呼ばれる負の連鎖で肥え太った貧乏神なのである
- 誰かがこの貧乏神との決別を提案したとしても誰かの「お前責任とれるのか？」のパワーワードがさらに貧乏を引き寄せ、そしてまた無駄にコストが嵩み、誰かの手によってプロジェクトが幸せになることを全力で阻止されるのである
- しかし、貧乏神は甘美でまるで美少女のような華やかさをもってあなたを誘惑する、誰もが貧乏神には勝てないのである、なぜなら貧乏神は美少女なのだから

## 貧乏神と決別しなければならない
- 「お前、初期コストに責任もてるのか？」
- 「そもそもお前、その後の技術負債で結局チャラになるねんで」
- そう、その通り、魅力的な開発初期における実装速度はまるで自分自身に羽が生えたかのような錯覚に陥るが、ロウで作った鳥の羽で調子にのったイカロスのように、泥船にのったタヌキのように、最後は鮮やかにその命を散らせる事になるのである
- 開発初期にかける設計コストは３匹のこぶたの末の弟のように狼に負けないレンガの家を作るかのように、賢い行いなのである
- さぁ、設計しよう
- 設計するのだ！！

## まずは導入するがいい

- Unity でプロジェクトを作り、ソースコードを編集するエディタを開くがいい
- Packages/manifest.json を開き、以下の項目を追記するがいい
- 色々と他に依存しているパッケージも大量に含まれているからごめんね

```json
    "jp.megamin.poppokoubou.library": "https://github.com/medamap/PoppoLibrary.git?path=Assets/PoppoKoubou#1.0.13",
    "jp.megamin.poppokoubou.library.demo": "https://github.com/medamap/PoppoLibrary.git?path=Assets/PoppoKoubou-Demo#1.0.13",
    "com.cysharp.messagepipe": "1.8.1",
    "com.cysharp.messagepipe.vcontainer": "1.8.1",
    "jp.megamin.messagepipe.interprocess": "https://github.com/medamap/MessagePipe.git?path=src/MessagePipe.Interprocess#feature/android_interprocess",
    "jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.16.8",
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
    "com.cysharp.zstring": "2.6.0",
    "org.nuget.messagepack": "3.1.3",
    "org.nuget.microsoft.net.stringtools": "1.0.0",
    "org.nuget.observablecollections.r3": "3.3.3",
    "org.nuget.r3": "1.2.9",
```

- そして manifest.json の後ろの部分に scopedRegistries を追記するがいい、手前の } のうしろにカンマを忘れないようにな

```json
  "scopedRegistries": [
    {
      "name": "Unity NuGet",
      "url": "https://unitynuget-registry.azurewebsites.net",
      "scopes": [
        "org.nuget"
      ],
      "overrideBuiltIns": false
    },
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.cysharp"
      ],
      "overrideBuiltIns": false
    }
  ]
```

## さぁ、使え

執筆中・・・































