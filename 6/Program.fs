// 単純な値の完全性
// これまでの方法
// type WidgetCode = WidgetCode of string // 先頭が"W"+数字4桁
// type UnitQuantity = UnitQuantity of int // 1以上1000以下
// type KilogramQuantity = KilogramQuantity of decimal // 0.05以上100.00以下


// module Domain = 
//     // 新しい方法
//     // スマートコンストラクタ
//     type UnitQuantity = private UnitQuantity of int

//     // 型と同じ名前のモジュールを定義
//     module UnitQuantity =
//         /// ユニット数の「コンストラクタ」を定義
//         /// int -> Result<UnitQuantity, string>
//         let create qty =
//             if qty < 1 then
//                 // 失敗
//                 Error "UnitQuantity can not be negative"
//             else if qty > 1000 then
//                 Error "UnitQuantity can not be greater than 1000"
//             else
//                 // 成功 -- 戻り値を構築
//                 Ok (UnitQuantity qty)
        
//         let value (UnitQuantity qty) = qty

// // 使用例
// // エラーが出るパターン
// module Main =
//     let unitQty = Domain.UnitQuantity.create  1

//     match unitQty with
//     | Error (msg: string) -> printfn "Failure, Message is %s" msg
//     | Ok unitQty -> 
//     printfn "Success, Value is %A" (Domain.UnitQuantity.value unitQty)
//     let innerValue = Domain.UnitQuantity.value unitQty 
//     printfn "Success, InnerValue is %A" innerValue

// 測定単位
[<Measure>] type kg
[<Measure>] type m

let fiveKilos = 5.0<kg>
let fiveMeters = 5.0<m>

// コンパイルエラー
// fiveKilos = fiveMeters

// kilogramQuantity<キログラム量が>が実際にキロであることを強制し、誤ってポンドの値で初期化できないようにする
// 型で表現するとこうなる
type KilogramQuantity = KilogramQuantity of decimal<kg>

// 型システムによる普遍条件の矯正
// 1つの注文には1つの明細行が必ず存在することを強制する
type NonEmptyList<'a> = {
    First: 'a
    Rest: 'a list
}

type OrderLine = exn

type Order = {
    OrderLines: NonEmptyList<OrderLine>
}

// ビジネスルールを型システムで表現する
// ある会社の顧客のメールアドレスを例に考える
// メールアドレスは検証済みの状態と未検証の状態がある
// メールアドレスには以下のようなルールがある
// ・検証メールは、未検証の電子メールアドレスにのみ送信してすべきです（既存の顧客へのスパム行為を避けるため）
// ・パスワードのリセットメールは、検証済みの電子メールアドレスにのみ送信してすべきです（不正なメールアドレスに送信すると迷惑メールとして扱われる可能性があるため）

// よく用いられる方法
// 検証が行われたかどうかを示すフラグを使うもの
// この方法にはいくつかの問題点がある
// フラグがいつ、なんのために設定され、解除されるのかが明確ではありません
// 例えば、顧客の電子メールアドレスが変更された場合、フラグはfalseに戻されるべきです
// しかし、以下の定義からはそのようなことはわかりません
// type CustomerEmail = {
//     EmailAddress: string
//     IsVerified: bool
// }

// これをモデル化するための方法
// 「検証済みのメールアドレス」と「未検証のメールアドレス」を定義する
// Q.メールアドレスの書式にはルールがある。それを定義するには？
// A.答えは以下の通り
// メールアドレスの型定義
// スマートコンストラクタにすることで後述の関数でしかメールアドレスを作れないようにする
type EmailAddress = private EmailAddress of string

// メールアドレスの書式を検証する正規表現（例）
let emailRegex = new System.Text.RegularExpressions.Regex(@"^.+@.+\..+$")

// ルールを定義する
let createEmailAddress (emailStr: string) : Result<EmailAddress, string> = 
    if emailRegex.IsMatch(emailStr) then
        Ok (EmailAddress emailStr)
    else
        Error "無効なメールアドレス形式です。"


printfn "成功するケース"
let ValidStr = "hoge@gmail.com"
printfn "--- 試行1: %s ---" ValidStr
let result1 = createEmailAddress ValidStr
// "Result"型はmatch式で処理する
match result1 with
    | Ok validEmail ->
        printfn "成功！検証済みのメールアドレスが作成されました：%A" validEmail
    | Error msg ->
        printfn "失敗...理由：%s" msg

printfn ""

printfn "失敗するケース"
let inValidStr = "hogegmail.com"
printfn "--- 試行2: %s ---" inValidStr
let result2 = createEmailAddress inValidStr
// "Result"型はmatch式で処理する
match result2 with
    | Ok validEmail ->
        printfn "成功！検証済みのメールアドレスが作成されました：%A" validEmail
    | Error msg ->
        printfn "失敗...理由：%s" msg

printfn ""



// CustomerEmailに誤って検証されてないメールアドレスを渡さないようにするため
// 「検証済みのメールアドレス」を定義する
// Q.「検証済みのメールアドレス」の定義はこれでいいのか？
// Q.検証ロジックはこれでいいのか？
type VerifiedEmailAddress = VerifiedEmailAddress of  EmailAddress
type CustomerEmail =
    | Unverified of EmailAddress
    | Verified of VerifiedEmailAddress

// リセットメール送信のワークフローを定義する
// Q.出力はどうなるだろう
// Q.この関数は「メールを送信する」という副作用がある？
// A.ある

// Q.副作用という言葉の使い方はこれであっているか？
// A.副作用の定義は以下の通り
// 副作用（Side Effect）とは、関数が「計算して値を返す」という本来の仕事以外で行う、外部へのあらゆる影響のこと。
// ・プログラムの世界における「外部」とは、以下のようなものを指します。
// ・データベースへの書き込み
// ・ファイルへの保存
// ・コンソール（画面）への文字の出力（printfnも副作用です）
// ・ネットワークを通じたメールの送信
// ・グローバル変数の変更

// 送信すべきメール」を表すデータ型
type EmailToSend = {
    To: string // 宛先
    Subject: string // 件名
    Body: string // 本文
}
// 「パスワードリセットメールを作成する」というロジック（副作用なし）
type CreatePasswordResetEmail = VerifiedEmailAddress -> EmailToSend
// --- 3. 「本物」のロジックの実装 ---
// 副作用のない、純粋な関数
let createPasswordResetEmail (verifiedEmail: VerifiedEmailAddress) : EmailToSend =
    // VerifiedEmailAddressから元のstringを取り出す
    let (VerifiedEmailAddress (EmailAddress emailStr)) = verifiedEmail
    
    { 
        To = emailStr
        Subject = "パスワードリセットのご案内"
        Body = "パスワードをリセットするには、... のリンクをクリックしてください。" 
    }
    
// では実際の「メールを送信する」ワークフローはどうなるか？
// これが実際にメールを送信する能力を表したワークフロー
type DispatchEmail = EmailToSend -> unit

// 全てのワークフローを組み合わせると「パスワードリセットのワークフロー」は全体でこうなる
type ResetPasswordworkflow = 
    CreatePasswordResetEmail -> DispatchEmail -> VerifiedEmailAddress -> unit

let resetPasswordWorkflow (createEmail: CreatePasswordResetEmail) (dispatch: DispatchEmail) (verifiedEmail: VerifiedEmailAddress) : unit =
// 1. 副作用のないロジックを呼び出し、「レシピ」データを作成
    let emailToSend = createEmail verifiedEmail
    
    // 2. 副作用のある実行系を呼び出し、「レシピ」を渡して実際に送信
    dispatch emailToSend

// --- ★ ここからが、あなたがリクエストしたテストコードです ★ ---

// 5. 「偽物（モック）」の dispatch 関数を定義
//    型シグネチャ `EmailToSend -> unit` さえ合っていればOK
let mockDispatchEmail (email: EmailToSend) : unit =
    printfn "--- ★[モック実行]★ ---"
    printfn "    実際にメールは送信せず、コンソールに出力します："
    printfn "    宛先: %s" email.To
    printfn "    件名: %s" email.Subject
    printfn "    本文: %s" email.Body
    printfn "--- ★[モック完了]★ ---"


// --- 6. テストの実行 ---
printfn "--- テスト開始 ---"

// テスト用のダミーデータを作成
let testEmail = EmailAddress "test-user@example.com"
let verifiedTestEmail = VerifiedEmailAddress testEmail

// ワークフローを実行
// 第1引数に「本物のロジック」 (createResetEmail)
// 第2引数に「偽物の実行系」 (mockDispatchEmail) を注入（DI）する
resetPasswordWorkflow createPasswordResetEmail mockDispatchEmail verifiedTestEmail

printfn "--- テスト終了 ---"