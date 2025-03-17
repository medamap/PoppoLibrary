# ぽっぽライブラリ
[![GitHub release](https://img.shields.io/github/v/release/medamap/PoppoLibrary)](https://github.com/medamap/PoppoLibrary/releases)
![GitHub license](https://img.shields.io/github/license/medamap/PoppoLibrary)
![GitHub downloads](https://img.shields.io/github/downloads/medamap/PoppoLibrary/total)

## 概要してください
- Unity でドメイン駆動オニオンアーキテクチャスタイルで開発するためのライブラリなのである

## 要約してください
- なるべくUnity固有の機能に依存したプログラムを書かない
- アプリケーションの役割の種類毎に分離し、ごっちゃに行き来するアプリは蜜結合となり、後になって技術負債で苦しむ事になるであろう
- ドメイン駆動とは、そのような苦しみを避けるための設計手法で、後になっても変更がしやすい設計を目指すものである
- 思い付きでクラスを作り、プロパティを追加し、SerializeField まみれのなんでも出来るスーパークラスは作ってはいけない
- 開発初期は面白いように進捗が進むのだが、後になってからの修正が困難になるのである

## 具体的におねがいします
- ネットワーク、データの保存、オブジェクトの操作、オブジェクトの描画、データの加工、アプリ固有の処理など、カテゴリ毎に明確に責任を分割し、ドメインとして定義する
- 各々のドメインは、お互い呼びあう事がないようにすることで、蜜結合を防ぎ、後になって苦しむ事が減るであろう
- ドメインの中もいくつかのレイヤーに分割し、それぞれのレイヤーが明確な責任を持つようにする
  - Presentation 層 ... MonoBehaviour を敬称したり、Unity 固有の機能に密接に結合が赦される、ここから直接アクセスが赦されるのは Application 層まで
  - Application 層 ... ドメインの中心にアクセスが許される層で、ここから Infrastructure 層または Domain 層へのアクセスが許されるが、基本的には Infrastructure 層へのアクセスが許される
  - Infrastructure 層 ... データの保存、ネットワーク、オブジェクトの操作など、具体的にやりたい事をここに記述する
  - Domain 層 ... 取り扱うデータの定義や Infrastructure 層で行う処理をインターフェイスとして定義を記述する

## 結論、このライブラリは
- このライブラリは、上記の考え方に基づいて実装されており、また、その実装をサポートするための機能を提供するものである

## 脳裏に蘇る Unity で苦しんだ記憶
- Unity で MonoBehaviour のシングルトンを覚えて狂喜乱舞した記憶はあるだろう
- これで明確に定義されたサービスに容易にアクセスでき、便利になったと使いまくった事は許しがたい黒歴史である
- そして、気が付けば SerializeField まみれ、シングルトンどうしに依存しあい、気が付けばどこか修正するとどこかに悪影響が、そしてまた増える SerializeField とシングルトン、もはやあなたはシングルトン地獄から逃れられない、どうだ怖くなってきただろう
- そしてやってくる応援の人員、彼は地獄のように依存の応酬のシングルトン天国に気が狂い、そして無駄に費やされる保守コスト、もはや誰も幸せになれない、そんな地獄、あなたはそんな開発環境にしたいのか？
- 「これ、壊してゼロから作り直そうぜ」誰かの一言に「いや、もうここに実績のあるシステムがあるんだ、お前責任とれるのか？」そして再び全員が地獄に飲み込まれていく、ちゃんちゃん

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

## さぁ、使おうか！！！！！！

- その前に、Unity プロジェクトを作らなければならない Unity のバージョンはここでは 6000.0.38f1 である
- プロジェクト作成時の指定は下記のように、適宜と書かれているところは適当に入れていただきたいのである

| 項目 | 設定 |
| --- | --- |
| Editor Version | 6000.0.38f1 |
| Project Template | 3D (Built-in Render Pipeline) |
| Project Name | NekoKawaii |
| Location | 適宜 |
| Unity Cloud Organization | 適宜 |
| Connect To | チェックは外す（つけてもいいけど外す） |
| Use Unity Version Control | チェックは外す（つけてもいいけど Git の方が良いであろう） |

- 必須項目入力が完了すれば「Create project」ボタンがクリック可能となる、さぁ、来たれ、ここに甘美なる世界が待っている

## まずは導入するがいい

- プロジェクトを無事作成できたら、人によっては延々と待たされたうえでついに Unity エディタがバシュン！と起動する、親の敵のかのようないつもの見慣れた画面が見れるであろう
- そして、プロジェクト作成直後に必ずやらなければいけない儀式を行う必要がある、落ち着いて聞いてほしい
- ソースコードを編集するエディタを開くのだ、あなたは何のエディタを使うのであろうか、おそらく殆どの方は VSCode を使うものと推察するが、残念ながらわたしは JetBrains の Rider だったりする、どうだ落胆したか？
- ところで、デフォルトのエディタってなんだろうか、昔は MonoDevelop とかいう絶望しか感じないいかにもオープンソースなエディタであった、彼は元気であろうか
- エディタは何を使うのかは Unity の [Edit] -> [Preferences] -> [External Tools] から設定できるのである、ここで設定しておくと便利である、もちろん JetBrains の Rider をセットしたよな？ なに？ メモ帳だと！？ そんなバカな！！負けた orz
- 閑話休題 Packages/manifest.json を開き、以下の項目を追記するがいい
- 色々と他に依存しているパッケージも大量に含まれているからごめんしてね

```json
    "jp.megamin.poppokoubou.library": "https://github.com/medamap/PoppoLibrary.git?path=Assets/PoppoKoubou#1.0.20",
    "jp.megamin.poppokoubou.library.demo": "https://github.com/medamap/PoppoLibrary.git?path=Assets/PoppoKoubou-Demo#1.0.20",
    "com.cysharp.messagepipe": "1.8.1",
    "com.cysharp.messagepipe.vcontainer": "1.8.1",
    "jp.megamin.messagepipe.interprocess": "https://github.com/medamap/MessagePipe.git?path=src/MessagePipe.Interprocess#1.8.1-extended.15",
    "jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.16.8",
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
    "com.cysharp.zstring": "2.6.0",
    "org.nuget.messagepack": "3.1.3",
    "org.nuget.microsoft.net.stringtools": "1.0.0",
    "org.nuget.observablecollections.r3": "3.3.3",
    "org.nuget.r3": "1.2.9",
```

- そして manifest.json の後ろの部分に scopedRegistries を追記するがいい、手前の `}` のうしろにカンマを忘れないようにな

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

- これらの記述を入れて Unity に戻るとギュンギュン回りだしてダウンロードしてセットアップを始めるので、あなたはビックリするかもしれない、だが安心して欲しい、失敗する時もたまにはあるであろう
- もしうまく行かないときはきっとダウンロードに失敗するであるとか Git がインストールされてないであるとか、プロキシに阻まれているとかいろいろあるに違いない、まぁ、たいていは Git がインストールされてないと怒られる事が大多数であろうから、せめてもの情けで Git のダウンロード先くらいは案内するのである
- [Git のダウンロード先](https://git-scm.com/downloads)
- 尚、プロキシとかそういった類の物の怪についてはこちらでは関知しないものとする、ご自分で検索して調べるか、ネットワーク担当者に泣きつくなどして、せいぜいがんばって解決していただきたい
- 無事、各種パッケージのダウンロードが完了したら、準備完了である、待ちきれない、早くゲームを作ろう、そういきりたつのもわかるのだが、まずは落ち着いてほしい
- さて Unity ちゃんが落ち着いたらパッケージが綺麗にインストールされているか確認してみようではないか [Window] -> [Package Manater] でインストールされたパッケージが諸々表示されるのだが、一番上あたりに Packages - Cysharp の R3(NuGet) あたりなんかが入っていれば多分大丈夫
- 次に Ctrl+Shift+C を押したらログウインドウが表示される、多分赤いマークのエラーが沢山あるだろう Console の左上の 「Clear」を押して全部消えれば多分大丈夫
- 残念ながら「Clear」を押しても消えない赤いエラーが残っていた場合、それはきっとパッケージのインストールの不具合であるかなんであるか、非常にやっかいな魔物に遭遇した可能性が高い
- 可能性としては、インストールされるべきパッケージが失敗してるとかなら再度 Unity エディタを立ち上げなおすとかでリロードしてもらって治る事もありうるのだが、そうでない場合は厄介である、その場合のケースはいろいろ多岐にわたるため、がんばって検索して解決していただきたい
- ありえる未来としてはこのドキュメントが記されて数年たち、作者がこのライブラリに対する興味を失い、メンテされなくなり、いろいろ関連ライブラリのバージョンアップが進んだり消えてしまったりなんやかんやで整合性が合わなくなった場合であるが、その時はもうあなたが個人でがんばるしかない、ファイト！！

## フォルダだ、フォルダを作ろう

- さて、いろいろあったのだが先ずは Assets フォルダにアプリ名のフォルダを作るが良い、ここは当然 NekoKawaii なのだ
- 最初にすることはアプリの起動だから、NekoKawaii の下に Boot フォルダを作るのだ、これをブートドメインと呼ぼう、そしてフォルダを作るのだ
- そして、その中に Presentation, Application, Infrastructure, Domain, LifetimeScope, Scenes のフォルダを作るのだ、これが Boot ドメインのレイヤーとなるのだ
- そして、NekoKawaii の下に LifetimeScope フォルダを作ることになる、以下にここまでの図を示そうではないか
- もし可能であれば、フォルダ作成やらファイル作成やらは IDE またはエディタ上で行うのが吉である、何気に Unity ちゃんはファイルを作るたびに、フォルダを作るたびにギュンギュン回りだしてビルドとやらをおっぱじめるので非常にかったるい、お父さんはそんな子に育てた覚えはありませんよ！！
- それにしても JetBrains Rider は昔は設定で更新保存するたびに Unity に変更を通知するという嫌な機能をオフに出来たのであるが、最近はそんな機能がなくなったのか、機能しなくなったのか、毎回細かく Unity ちゃんに通知しやがるものですから、ちょっとでもフォーカスを Unity ちゃんに移す度に Unity ちゃんがギュンギュンと更新を初めて非常にウザったく思ったものである、自動更新は便利であるが、度を越すとエクスペリエンスとやらが低下するので多少不便であっても Ctrl+R で手動リフレッシュの心を大事にしたいものである

```
Assets/
├ NekoKawaii/
│　├ Boot/
│　│　├ Presentation/
│　│　├ Application/
│　│　├ Infrastructure/
│　│　├ Domain/
│　│　├ LifetimeScope/
│　│　└ Scenes/
│　└ LifetimeScope/
```

## VContainer 登場！！

- VContainer とは依存注入なのである、依存という言葉がヤバそうな雰囲気であるが、気にしないのである
- つまり、依存させたい成分をクラスの中に注射^H^H注入するのである、これだけではサッパリであろう、意味がわからんと思うのでひとまずおいておくのである
- このフレームワークは VContainer に依存しているので、VContainer ファーストで説明を進める事にしよう、兎にも角にも VContainer である
- 先ほど作った LifetimeScope フォルダに NekoKawaiiLifetimeScope.cs を作るのだ、そして以下のように書くのだ

```csharp
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.LifetimeScope;
using PoppoKoubou.CommonLibrary.Network.LifetimeScope;
using PoppoKoubou.CommonLibrary.UI.LifetimeScope;

public class NekoKawaiiLifetimeScope : VContainer.Unity.LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log($"NekoKawaiiLifetimeScope.Configure()");
        var options = builder.RegisterMessagePipe();

        // メッセージ登録
        this.AddAggregateServiceMessage(builder, options);
        this.AddLogMessage(builder, options);
        this.AddNetworkMessage(builder, options);
        this.AddUIMessage(builder, options);

        // コンポーネント登録
        this.AddAggregateServiceComponent(builder);
        this.AddLogComponent(builder);
        this.AddNetworkComponent(builder);
        this.AddUIComponent(builder);
            
        // エントリポイント登録
        this.AddAggregateServiceEntryPoint(builder);
        this.AddLogEntryPoint(builder);
        this.AddNetworkEntryPoint(builder);
        this.AddUIEntryPoint(builder);
    }
}
```

- さっそく逃げ出したくなるかのようなわけのわからない「おまじない」が沢山込められているのだが、これらは VContainer 界における Using や Include みたいなものだと思っておいて欲しい、後々説明するのである
- 次に、図に示した通りにソースコードのファイルを追加していって欲しい、詳しい説明は例によって後回しである

```
Assets/
├ NekoKawaii/
│　├ Boot/
│　│　├ Presentation/
│　│　├ Application/
│　│　│　└ BootService.cs ← NEW!!
│　│　├ Infrastructure/
│　│　├ Domain/
│　│　├ LifetimeScope/
│　│　│　└ BootLifetimeScopeExtensions.cs ← NEW!!
│　│　└ Scenes/
│　└ LifetimeScope/
│　　　└ NekoKawaiiLifetimeScope.cs
```

- 先ずは BootService.cs 

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using VContainer;

namespace NekoKawaii.Boot.Application
{
    /// <summary>ブートサービス</summary>
    [Inject] public class BootService : ServiceNode
    {
        /// <summary>依存注入</summary>
        public BootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher)
            : base(
                "ブートサービス",
                100,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.AddLine($"BootService.StartInitialize()", LogLevel.Debug);
            await UniTask.Delay(1, cancellationToken: ct);
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.AddLine($"BootService.StartService()", LogLevel.Debug);
            await UniTask.Delay(1, cancellationToken: ct);
        }

        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.AddLine($"BootService.Dispose()", LogLevel.Debug);
        }
    }
}
```

- 説明は後回しにして BootLifetimeScopeExtensions.cs

```csharp
using MessagePipe;
using NekoKawaii.Boot.Application;
using VContainer;
using VContainer.Unity;

namespace NekoKawaii.Boot.LifetimeScope
{
    public static class BootLifetimeScopeExtensions
    {
        public static void AddBootMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
        }
        public static void AddBootComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
        }
        public static void AddBootEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
            builder.RegisterEntryPoint<BootService>();
        }
    }
}
```

- 次に、最初に作った Assets/NekoKawaii/LifetimeScope/NekoKawaiiLifetimeScope.cs を以下のように修正する

```csharp
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.LifetimeScope;
using PoppoKoubou.CommonLibrary.Network.LifetimeScope;
using PoppoKoubou.CommonLibrary.UI.LifetimeScope;
using NekoKawaii.Boot.LifetimeScope; // ← NEW!!

public class NekoKawaiiLifetimeScope : VContainer.Unity.LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log($"NekoKawaiiLifetimeScope.Configure()");
        var options = builder.RegisterMessagePipe();

        // メッセージ登録
        this.AddAggregateServiceMessage(builder, options);
        this.AddLogMessage(builder, options);
        this.AddNetworkMessage(builder, options);
        this.AddUIMessage(builder, options);
        // NekoKawaiiメッセージ登録 ← NEW!!
        this.AddNekoKawaiiMessage(builder, options); // ← NEW!!

        // コンポーネント登録
        this.AddAggregateServiceComponent(builder);
        this.AddLogComponent(builder);
        this.AddNetworkComponent(builder);
        this.AddUIComponent(builder);
        // NekoKawaiiコンポーネント登録 ← NEW!!
        this.AddNekoKawaiiComponent(builder); // ← NEW!!
            
        // エントリポイント登録
        this.AddAggregateServiceEntryPoint(builder);
        this.AddLogEntryPoint(builder);
        this.AddNetworkEntryPoint(builder);
        this.AddUIEntryPoint(builder);
        // NekoKawaiiエントリポイント登録 ← NEW!!
        this.AddNekoKawaiiEntryPoint(builder); // ← NEW!!
    }
}
```

- どうであろうか？ おまじないをわけのわからないまま言われるがままに書き写していくのはさぞかしこそばゆいものであろう、しかし、まだまだ説明は後回しである、サクサクと次へ進もうではないか
- 次に、先ほど最初に作った Assets/NekoKawaii/Boot/Appliation/BootService.cs を以下のように修正する

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using VContainer;

namespace NekoKawaii.Boot.Application
{
    /// <summary>ブートサービス</summary>
    public class BootService : ServiceNode
    {
        /// <summary>依存注入</summary>
        [Inject] public BootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher)
            : base(
                "ブートサービス",
                100,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.AddLine($"BootService.StartInitialize()", LogLevel.Debug);
            await UniTask.Delay(1, cancellationToken: ct);
            LogPublisher.AddLine($"初期化したのだぜ", LogLevel.Info, "#ffff00"); // ← NEW
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.AddLine($"BootService.StartService()", LogLevel.Debug);
            await UniTask.Delay(1, cancellationToken: ct);
            // CancellationTokenでキャンセルされるまで無限ループ ← NEW
            while (!ct.IsCancellationRequested) // ← NEW
            { // ← NEW
                LogPublisher.AddLine($"サービス中", LogLevel.Info, "#00ff00"); // ← NEW
                await UniTask.Delay(1000, cancellationToken: ct); // ← NEW
            } // ← NEW
        }

        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.AddLine($"BootService.Dispose()", LogLevel.Debug);
        }
    }
}
```

- 次は Unity エディタでの作業である、Hierarchy（ヒエラルキ）に空の GameObject を作成し、名前を NekoKawaiiLifetimeScope とし Assets/NekoKawaii/LifetimeScope/NekoKawaiiLifetimeScope.cs をアタッチ
- さて、それではそろそろ頃合いである、Unity エディタで再生ボタンを押してプログラムを実行してみよう、今回はビジュアル的なものはなく、定番の Debug.Log() 的なやつである、実行すると最初になにやらカラフルなシステム初期化ログが流れ、そして「サービス中」という緑色の文字が１秒おきに延々と流れるのである

## 説明

- 今回ヒエラルキに配置した NekoKawaiiLifetimeScope は VContainer のライフタイムスコープである、これは VContainer において依存関係を管理するためのものであるがエントリポイントとして登録したクラスを起動させる役割も担っている
- 今回の例では BootService というクラスをエントリポイントとして登録しているのだが、その場所は下図の BootLifetimeScopeExtensions.cs の中に書かれている
- そして、ヒエラルキに登録した NekoKawaiiLifetimeScope.cs の中で、BootLifetimeScopeExtensions.cs の中で定義した拡張メソッドを呼び出し、結果的にエントリポイントが登録されたのである

```
Assets/
├ NekoKawaii/
│　├ Boot/
│　│　├ Presentation/
│　│　├ Application/
│　│　│　└ BootService.cs ... (3) そして、NekoKawaiiLifetimeScope.cs が初期化されるとコレが MonoBehaviour ぽくふるまい始めるのである
│　│　├ Infrastructure/
│　│　├ Domain/
│　│　├ LifetimeScope/
│　│　│　└ BootLifetimeScopeExtensions.cs ... (1) ここでエントリポイント登録の実体がある
│　│　└ Scenes/
│　└ LifetimeScope/
│　　　└ NekoKawaiiLifetimeScope.cs ... (2) ここから(1)を呼び出してエントリポイント登録を呼び出ししている
```
- エントリポイントの登録とは

```csharp
builder.RegisterEntryPoint<BootService>();
```

- になるわけだが、<>の中に書かれたクラスがエントリポイントとなる

## エントリポイントとは VContainer 版 MonoBehaviour である

- くどくど言うより結論から先に言うと、エントリポイントとは VContainer 版の MonoBehaviour である、つまり、MonoBehaviour が Unity のライフサイクルに従って動作するのに対して、エントリポイントは VContainer のライフサイクルに従って動作するのである
- つまり、ねっとりとなっているが、下記クラスにコメントとして説明をいれておいた、読むように

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using VContainer;

namespace NekoKawaii.Boot.Application
{
    /// <summary>ブートサービス</summary>
    public class BootService : ServiceNode
    // BootService が継承している ServiceNode サービスノードはこのフレームワーク独自の機能として用意した
    // サービスノードの定義を辿ると VContainer が初期化するインターフェイス IAsyncStartable インターフェイスを
    // サービスノード内で実装されている
    // IAsyncStartable は public async UniTask StartAsync(CancellationToken ct) を実装する
    // VContainer で エントリポイントとして登録されると IAsyncStartable.StartAsync() が呼び出される
    // そして、サービスノードの中から他のサービスノードとの優先順位を考慮しながら継承したクラスの
    // StartInitialize() と StartService() が呼び出されるといった感じである
    // つまり、StartInitialize() と StartService() は IAsyncStartable.StartAsync() と地続きなのだ
    // そして VContainer のエントリポイントは実は他にもいろいろなインターフェイスが用意されている
    // 
    // IInitializable     ... Initialize()     // コンテナを構築した直後に呼び出される
    // IPostInitializable ... PostInitialize() // 上記より遅いタイミングで呼び出される
    // IStartable         ... Start()          // ほぼ MonoBehaviour.Start()
    // IAsyncStartable    ... StartAsync()     // ほぼ MonoBehaviour.Start()
    // IPostStartable     ... PostStart()      // MonoBehaviour.Start() より遅め
    // IFixedTickable     ... FixedTick()      // ほぼ MonoBehaviour.FixedUpdate()
    // IPostFixedTickable ... PostFixedTick()  // MonoBehaviour.FixedUpdate() より遅め
    // ITickable          ... Tick()           // ほぼ MonoBehaviour.Update()
    // IPostTickable      ... PostTick()       // MonoBehaviour.Update() より遅め
    // ILateTickable      ... LateTick()       // ほぼ MonoBehaviour.LateUpdate()
    // IPostLateTickable  ... PostLateTick()   // MonoBehaviour.LateUpdate() より遅め
    // 詳しくは https://vcontainer.hadashikick.jp/ja/integrations/entrypoint を参照の事
    //
    // ところでだが、ここで ServiceNode を継承せずに、上記インターフェイスを実装するでも構わない
    // ServiceNode の役割は決められた順番で初期化をする時に使われるだけなので、それらの機能が不要であればむしろ邪魔かもしれないのだから
    // MonoBehaviour のシングルトンを量産するうちに初期化の順番を制御する必要が出てきた事は誰でも経験があると思う、
    // その悩みを解決するのがこの ServiceNode なのである
    {
        /// <summary>依存注入</summary>
        // ここでは VContainer の本領発揮される箇所である、所謂コンストラクタであるが [Inject] がついているのがチャームポイントである
        // VContainer はこの [Inject] がついているコンストラクタを優先的に呼び出し、そして、コンストラクタ引数のクラスやインターフェイスを
        // 自動的にインスタンスを作成し、ぶっこんでくれる、下記の場合
        //     logPublisher には IPublisher<LogMessage> を実装したインスタンスがやってくる
        //     centralHubStatusSubscriber には ISubscriber<CentralHubStatus> を実装したインスタンスがやってくる
        //     serviceNodeStatusPublisher には IPublisher<ServiceNodeStatus> を実装したインスタンスがやってくる
        // という感じである
        // つまり、これが、「依存注入」なのである、依存注入されたインスタンスは、VContainer が保持しており、他のエントリポイント
        // 等でも注入される場合、最初に作成され保持されたインスタンスがつっこまれる、つまり、シングルトンである
        // ちなみに、依存注入できるインスタンスは全部が全部シングルトンではなく、毎回新しいインスタンスが作成される場合もある
        // なお [Inject] がついているコンストラクタで指定されている引数は全て LifetimeScope　にてあらかじめ登録しておかないと
        // エラーになるので注意すること、そして、登録時にシングルトンにするか、毎回インスタンス生成するかは選択可能である
        [Inject] public BootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher
            // もし、このクラスに更に注入したいクラスやインターフェイスがあれば、ここに追加していく
            // 何が指定されても LifetimeScope で登録されたものであれば VContainer は適宜インスタンスを作成して突っ込んでくれる
            // VContainer は寿司屋の板前のようなものである、注文されたら適宜作成して提供してくれるのである
            // 唯一寿司屋と違うのは シングルトンは誰が注文したとしても最初に１つしたものをみんなで共有してもらうということである
            // どうであろうか MonoBehaviour で苦労してシングルトンの仕組みを実装した苦労がここで無くなる事に気が付いただろうか？
            // VContainer はシングルトンの仕組みを実装しないただのクラスであってもシングルトンにしてくれるのだ、どうだ、すごいだろう
        ) : base(
            "ブートサービス", // ここはエントリポイントにつけられた名前を自由に命名するところ
            100, // ここはエントリポイントの優先度を設定するところで、初期化の順序を決める、数値が小さい程優先度が高い
            logPublisher, // ログ出力に使われるオブジェクトでサービスノード固有の機能だが、このクラス内でも使用できる
            centralHubStatusSubscriber, // サービスノード内で優先順位に従って初期化するために使われている
            serviceNodeStatusPublisher) // サービスノード内で優先順位に従って初期化するために使われている
        {
            // ここでは依存注入されたインスタンスをクラス内の変数に代入しているだけである
            // とはいっても、このクラスでは全部親クラスにもっていかれてるので何も記述していないが
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            // ここは Unity における Awake() みたいなものである
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            // ここは Unity における Start() みたいなものである
        }

        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            // ここは Unity における OnDestroy() みたいなものである
        }
    }
}
```

## 続きはまた来週

- 非常にわかりにくい説明であっただろうか
- 本来の VContainer の基本をすっとばしていきなり応用から始まる無茶苦茶なドキュメントなのであるが、最終的には VContainer 以外の機能も色々ふんだんに贅沢にたくさん盛り込んでいってモリモリにしてみようと思っている
- 清く正しく美しく基本から学びたい方は本家を読んで勉強されたし
- 公式は [VContainer](https://vcontainer.hadashikick.jp/ja/) である、公式の資料は素晴らしい、是非これを読んで VContainer の素晴らしさを体験して欲しい
