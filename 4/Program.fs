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
type PaymentAmount = PaymentAmount of decimal

// 通過
type Currency =
    | EUR
    | USD

// 支払い型
// 支払い総額と通貨の直積型で表す
type Payment =
    { Amount: PaymentAmount
      Currency: Currency
      Method: PaymentMethod }


// 請求ID
type InvoiceId = InvoiceId of int

// 顧客情報
type CustomerInfo =
    { Name: string; BiilingAddress: string }

// 請求額
type BilledAmount =
    { Amount: PaymentAmount
      Currency: Currency }

// 請求項目リスト
type InvoiceLine =
    { Description: string; Amount: decimal }

// UnpaidInvoiceの定義は？
// 未払いの請求書
type UnpaidInvoice =
    { Id: InvoiceId
      CustomerInfo: CustomerInfo
      AmouuntDue: BilledAmount
      Lines: InvoiceLine list }


// PaidInvoiceの定義は
// 支払い済みの請求書
// 元の請求書と支払い情報を組み合わせたレコード
type PaidInvoice =
    { OriginalInvoice: UnpaidInvoice
      Payment: Payment
      PaidOnDate: System.DateTime
      ConfirmationCode: string }

// 実行可能なアクションを文章化するために、代わりに関数を表す型を定義する
type PayInvoice = UnpaidInvoice -> Payment -> PaidInvoice

// アクションの具体的な実装例
let payInvoice (unpaid: UnpaidInvoice) (payment: Payment) : PaidInvoice =
    { OriginalInvoice = unpaid
      Payment = payment
      PaidOnDate = System.DateTime.UtcNow
      ConfirmationCode = "CONF-XYZ-123" }

// 1. サンプルの「未払いの請求書」データを作成する
let sampleUnpaidInvoice =
    { Id = InvoiceId 101
      CustomerInfo =
        { Name = "山田 太郎"
          BiilingAddress = "東京都渋谷区..." }
      AmouuntDue =
        { Amount = PaymentAmount 1500.00m
          Currency = USD }
      Lines =
        [ { Description = "商品A"
            Amount = 1000.00m }
          { Description = "商品B"
            Amount = 500.00m } ] }

// 2. サンプルの「支払い」データを作成する
let samplePayment =
    { Amount = PaymentAmount 1500.00m
      Currency = USD
      Method =
        Card
            { CardType = Visa
              CardNumber = CardNumber "1234-5678-9012-3456" } }

// 3. payInvoice関数を呼び出して、未払いの請求書を支払う
let resultingPaidInvoice = payInvoice sampleUnpaidInvoice samplePayment

// 4. 結果として得られた「支払い済みの請求書」をコンソールに表示する
printfn "--- 支払い処理が完了しました ---"
printfn "作成された「支払い済み請求書」:"
// `%A` はデータ構造を人間が読みやすい形できれいに表示してくれる
printfn "%A" resultingPaidInvoice
