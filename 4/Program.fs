let add1 x = x + 1 // 型シグネチャ: int -> int
printfn "add1 10 = %d" (add1 10)

let add x y = x + y // 型シグネチャ: int -> int -> int
printfn "add 10 20 = %d" (add 10 20)

let squarePlusOne x = let square = x * x in square + 1 //  型シグネチャ: int -> int
printfn "squarePlusOne 10 = %d" (squarePlusOne 10)

// areEqual : `a -> `a -> bool
// アポストロフィがあると、ジェネリック型を表す
let areEqual x y = x = y
printfn "areEqual 10 10 = %b" (areEqual 10 10)


// type AppleVariety = {
//     Name: string
//     Color: string
// }

// type BananaVariety = {
//     Name: string
//     Color: string
// }

// type CherryVariety = {
//     Name: string
//     Color: string
// }

type AppleVariety =
    | GoldenDelicious
    | GrannySmith

type BananaVariety =
    | Yellow
    | Red

type CherryVariety =
    | Sweet
    | Sour

// レコード型
// レコード型は、複数のフィールドを持つことができる
// And型
type FruitSalad =
    { Apple: AppleVariety
      Banana: BananaVariety
      Cherryies: CherryVariety }

// Or型
// 縦棒はそれぞれの選択肢を区切っている
// このような選択型を判別共用体という
type FruitSnack =
    | Apple of AppleVariety
    | Banana of BananaVariety
    | Cherries of CherryVariety

// OR型の使い方
// match式を使う

let myApple = GoldenDelicious
let myBanana = Red
let myCherries = Sweet

// バナナの種類に応じて、説明文を返す関数
let describeBanana (banana: BananaVariety) =
    match banana with
    | Yellow -> "Yellow Banana"
    | Red -> "Red Banana"

printfn "describeBanana  = %s" (describeBanana myBanana)

// F#の型を扱う
type Person = { First: string; Last: string }

let aPerson = { First = "John"; Last = "Doe" }

printfn "aPerson = %s %s" aPerson.First aPerson.Last

// パターンマッチを使う
let { First = first; Last = last } = aPerson

// 判別共用体
// 判別共用体は、複数の選択肢を持つことができる
// このような選択型を判別共用体という
type OrderQuantity =
    | UnitQuantity of int
    | KilogramQuantity of decimal

let anOrderQuantity = UnitQuantity 10
let anOrderQuantity2 = KilogramQuantity 10.0m

// 選択型を分解する
let printQuantity anOrderQty =
    match anOrderQty with
    | UnitQuantity uQty -> printfn "UnitQuantity: %d" uQty
    | KilogramQuantity kQty -> printfn "KilogramQuantity: %f" kQty

printQuantity anOrderQuantity
printQuantity anOrderQuantity2

// 型の合成によるドメインモデルの構築
// Eコマースサイトの支払いを追跡するためのドメインモデルを構築する

// 小切手番号を表す型
type CheckNumber = CheckNumber of int
// カード番号を表す型
type CardNumber = CardNumber of string

// カードの種類を表す型
type CardType =
    | Visa
    | Mastercard // OR型 直和型やタグ付き共用体、判別共用体と呼ばれる

// クレジットカードを表す型
type CreditCardInfo = // AND型 直積型
    { CardType: CardType
      CardNumber: CardNumber }

// 支払い手段を表す型
type PaymentMethod =
    | Cash
    | Check of CheckNumber
    | Card of CreditCardInfo

// 支払い総額
// type PaymentAmount = PaymentAmount of decimal

// 通過
type Currency =
    | EUR
    | USD

// 支払い型
// 支払い総額と通貨の直積型で表す
// type Payment =
//     { Amount: PaymentAmount
//       Currency: Currency
//       Method: PaymentMethod }
type Payment = { Amount: decimal }

// 請求ID
type InvoiceId = InvoiceId of int

// 顧客情報
type CustomerInfo =
    { Name: string; BiilingAddress: string }

// 請求額
// type BilledAmount =
//     { Amount: PaymentAmount
//       Currency: Currency }

// 請求項目リスト
type InvoiceLine =
    { Description: string; Amount: decimal }

// UnpaidInvoiceの定義は？
// 未払いの請求書
type UnpaidInvoice = { Id: int; AmouuntDue: decimal }


// PaidInvoiceの定義は
// 支払い済みの請求書
// 元の請求書と支払い情報を組み合わせたレコード
// type PaidInvoice =
//     { OriginalInvoice: UnpaidInvoice
//       Payment: Payment
//       PaidOnDate: System.DateTime
//       ConfirmationCode: string }

// 実行可能なアクションを文章化するために、代わりに関数を表す型を定義する
// type PayInvoice = UnpaidInvoice -> Payment -> PaidInvoice

// A.通常の支払い処理
// let payInvoice (unpaid: UnpaidInvoice) (payment: Payment) : PaidInvoice =
//     { OriginalInvoice = unpaid
//       Payment = payment
//       PaidOnDate = System.DateTime.UtcNow
//       ConfirmationCode = "CONF-XYZ-123" }

// B.ポイントを使って支払う、別の実装
// let payInvoiceWithPoints (unpaid: UnpaidInvoice) (payment: Payment) : PaidInvoice =
// ポイント利用のロジック
// printfn " （ポイント利用のロジックが実行されました）"

// { OriginalInvoice = unpaid
//   Payment = payment
//   PaidOnDate = System.DateTime.UtcNow
//   ConfirmationCode = "CONF-XYZ-456" }

// 「請求書を支払う能力」を受け取って、ログ出力などの共通処理を追加する関数
// let executePaymentProcess (paymentProcessor: PayInvoice) (invoice: UnpaidInvoice) (payment: Payment) =
//     printfn "--- 支払いプロセスを開始します ---"
//     let paidInvoice = paymentProcessor invoice payment
//     printfn "--- 支払いプロセスが完了しました ---"
//     paidInvoice


// let sampleUnpaidInvoice =
//     { Id = InvoiceId 101
//       CustomerInfo =
//         { Name = "山田 太郎"
//           BiilingAddress = "東京都渋谷区..." }
//       AmouuntDue =
//         { Amount = PaymentAmount 1500.00m
//           Currency = USD }
//       Lines =
//         [ { Description = "商品A"
//             Amount = 1000.00m }
//           { Description = "商品B"
//             Amount = 500.00m } ] }

// // 2. サンプルの「支払い」データを作成する
// let samplePayment =
//     { Amount = PaymentAmount 1500.00m
//       Currency = USD
//       Method =
//         Card
//             { CardType = Visa
//               CardNumber = CardNumber "1234-5678-9012-3456" } }


// // 1. 「通常の支払い能力」を渡してプロセスを実行
// printfn "¥n[実行例1: 通常の支払い]"
// let result1 = executePaymentProcess payInvoice sampleUnpaidInvoice samplePayment
// printfn "結果：%A" result1

// // 2. 「ポイント利用の支払い能力」を渡してプロセスを実行
// printfn "¥n[実行例2: ポイントを使った支払い]"

// let result2 =
//     executePaymentProcess payInvoiceWithPoints sampleUnpaidInvoice samplePayment

// printfn "結果：%A" result2

// ではポイントの前にクーポンを適用するかどうか？をどうやって定義するか
// 支払い前に適用できるものが増えると、 その組み合わせのカ数だけアクションの定義が増えてしまう
// 例えば以下のようなアクションは自由度が低い
// type  PayInvoice = UnpaidInvoice -> Point -> Payment -> PaidInvoice
// それに対するアンサーとして以下のような定義が考えられる
// ポイントの型
// type Points = Points of int

// ポイント適用後の請求書、という新しい「状態」を表す型
// 元の請求書、使ったポイント、そして新しい請求額を持つ
// type InvoiceWithPointsApplied =
//     { OriginalInvoice: UnpaidInvoice
//       PointsUsed: Points
//       NewAmountDue: BilledAmount }

// 「支払い要求」は、「通常の請求」か「ポイント適用後の請求」のどちらかである
// type PaymentRequest =
//     | Regular of UnpaidInvoice
//     | WithPoints of InvoiceWithPointsApplied

// これは支払いにポイントが必ず使われる想定の定義となる
// ポイントを使うかどうか任意としたいというの常である
// この定義はポイント以外の適用できるものが増えると
// With〇〇AndXXみたいな定義どんどん増える

// できれば型を合成するような形で自由に定義をしたい
// それに対するアンサーとして「状態を変化させる（イベント）のリストを定義する
// 割引のモデルを定義
type Coupon = { Code: string; Amount: decimal }
type Points = Points of int

// 「割引」とは「クーポン利用」または「ポイント利用」である
// 「割引」を定義すると以下のようになる
type Discount =
    | CouponApplied of Coupon // 「クーポンの利用」を定義
    | PointsUesd of Points // 「ポイント利用」を定義
// GifCartApplied of GitCard // 新しい割引方法もこのように追加できる

type PaidInvoice =
    { InvoiceId: int
      AmountPaid: decimal
      AppliedDiscounts: Discount list }

// 「支払い前の請求書」という、進行中の状態を表す型を定義する
// 割引の「原因と過程」を残すと何が嬉しいの？
// ⇨その請求書がどういう過程でその金額になったかを記録するため
// 実際のお店のレシートのと同じ
// CurrentAmountDue (支払総額): レシートの一番下に書かれている、あなたが最終的に支払う金額。これさえあれば支払いはできる。
// AppliedDiscounts (割引リスト): 「会員割引: -100円」「クーポン利用: -200円」といった、支払総額に至るまでの内訳になる。
type PrePaymentInvoice =
    { OriginalInvoice: UnpaidInvoice
      AppliedDiscounts: Discount list // 割引の「原因と過程」を表す
      CurrentAmountDue: decimal } // 割引後の「結果」を表す

// 「状態を変化させる関数」を定義する
let applyCoupon (coupon: Coupon) (invoice: PrePaymentInvoice) : PrePaymentInvoice =
    {
      // これどういう意味
      // 「with」を使うと更新が必要な値だけコピーして新しいレコードが疲れる
      // この場合、OriginalInvoiceはそのままの値になる
      // invoiceの新しいコピーを作成する
      // コピーのAppliedDiscountsフィールドを更新する（元のリストの先頭に、新しい割引を1つ追加）
      // コピーのCurrentAountDueフィールドを更新する
      invoice with
        AppliedDiscounts = (CouponApplied coupon) :: invoice.AppliedDiscounts
        CurrentAmountDue = invoice.CurrentAmountDue - coupon.Amount }

let applyPoints points (invoice: PrePaymentInvoice) =
    let (Points p) = points
    let discountAmount = decimal p

    { invoice with
        AppliedDiscounts = (PointsUesd points) :: invoice.AppliedDiscounts
        CurrentAmountDue = invoice.CurrentAmountDue - discountAmount }

// TODO ステップ4：PayInvoiceの定義を更新するを書く
// 「支払い能力」は、割引適用済みの請求書(PrePaymentInvoice)を受け取るように更新する
type PayInvoice = PrePaymentInvoice -> Payment -> PaidInvoice

// 支払い関数の「実装」
let payInvoice (prePaymentInvoice: PrePaymentInvoice) (payment: Payment) : PaidInvoice =
    // 本来はここで、prePaymentInvoice.CurrentAmountDue と Payment.Amount が
    // 一致するかどうかを厳密に検証する
    if prePaymentInvoice.CurrentAmountDue <> payment.Amount then
        // エラー処理（この例では例外を投げる）
        failwithf "支払額が一致しません。請求額: %M, 支払額; %M" prePaymentInvoice.CurrentAmountDue payment.Amount
    else
        { InvoiceId = prePaymentInvoice.OriginalInvoice.Id
          AmountPaid = payment.Amount
          AppliedDiscounts = prePaymentInvoice.AppliedDiscounts }

// ここからが実行部分
// 1.元となる「未払い請求書」を作成
let unpaid = { Id = 101; AmouuntDue = 1500m }
printfn "--- 処理開始 ---"
printfn "元の請求額: %M¥n" unpaid.AmouuntDue

// 2.支払い前の請求書（割引の買い物カゴ）を初期化
let initialPrePayment =
    { OriginalInvoice = unpaid
      AppliedDiscounts = []
      CurrentAmountDue = unpaid.AmouuntDue }

// 3.クーポンを適用
let coupon = { Code = "AUTUMN2025"; Amount = 100m }
let afterCoupon = applyCoupon coupon initialPrePayment
printfn "クーポンを適用後・・・現在の請求額: %M¥n" afterCoupon.CurrentAmountDue

// 4.続いてポイントを適用
let points = Points 200
let afterPoints = applyPoints points afterCoupon
printfn "ポイント適用後・・・現在の請求額: %M¥n" afterPoints.CurrentAmountDue

// 5.最終的な請求額に合った「支払い」を作成
let finalPayment = { Amount = 1200m }
printfn "最終請求額 %M に対して、%Mを支払います・・・" finalPayment.Amount afterPoints.CurrentAmountDue

// 6.payInvoice関数を呼び出して支払いを完了
let paidInvoice = payInvoice afterPoints finalPayment
printfn "¥n支払い成功！"

// 7.最終的に作成された「支払いずみ請求書」の中身を表示
printfn "--- 作成された支払い済み請求書 ---"
printfn "%A" paidInvoice

// 8.最終的な請求額に合った「失敗する金額の支払い」を作成
let finalFailurePayment = { Amount = 1100m }
printfn "最終請求額 %M に対して、%Mを支払います・・・" finalFailurePayment.Amount afterPoints.CurrentAmountDue

// 9.payInvoice関数を呼び出して支払いを完了
let paidFailureInvoice = payInvoice afterPoints finalFailurePayment
printfn "¥n支払い失敗！"

// 10.最終的に作成された「支払いずみ請求書」の中身を表示
printfn "--- 作成された支払い済み請求書 ---"
printfn "%A" paidFailureInvoice


// 省略可能な値のモデリング
type Option<'a> =
    | Some of 'a
    | None

type PersonalName =
    { FirstName: string
      MiddleInitial: string option
      LastName: string }

// エラーのモデリング
type Result<'Success, 'Failure> =
    | Ok of 'Success
    | Error of 'Failure

// 請求書の支払いが失敗した時のエラー
type PaymentError =
    | CardTypeNotRecognized
    | PaymentRejected
    | PaymentProviderOffline

type payInvoiceWithError = UnpaidInvoice -> Payment -> Result<PaidInvoice, PaymentError>
