// 注文確定ワークフローを考える
// ワークフローの入力

// 顧客情報は適当
type CustomerInfo = {
    Name: string
    Emai: string
}
// 未検証の顧客情報
type UnvalidatedCustomerInfo = CustomerInfo of UnvalidatedCustomerInfo


// 配送先の住所
type Address = {
    City: string
    PostalCode: string
}
// 未検証の配送先
type UnvalidatedAddress = Address of UnvalidatedAddress

// 未検証の注文
// type UnvalidatedOrder = {
//     OrderId: string
//     CustomerInfo: UnvalidatedCustomerInfo
//     ShippingAddress: UnvalidatedAddress
// }
// ジェネリクスによる共通構造の共有
// 共通のフィールドはジェネリクスで共有する
type Command<'data> = {
    Data: 'data
    Timestamp: string
    UserId: string
    // etc
}

// 入力としてのコマンド
// 注文を確定する
// ワークフローの本当の入力は実際には注文書ではなくコマンド
// type PlaceOrder = Command<UnvalidatedOrder>
// type ChangeOrder = Command<UnvalidatedOrder>
// type CancelOrder = Command<UnvalidatedOrder>

// 複数のコマンドを一つの型にまとめる
// 境界づけられたコンテキストの全てのコマンドが同じ入力チャネルで送信されることもある
// それらをシリアライズできる1つのデータ構造に統一する
// type OrderTakingCommand = 
//     | Place of PlaceOrder
//     | Change of ChangeOrder
//     | Cancel of CancelOrder

// 状態の集合による注文のモデリング
// ＜未処理の注文書＞⇨＜未検証の注文＞⇨＜検証済みの注文＞⇨＜価格計算済みの注文＞
// ↓                                   ↓
// ⇨＜未検証の見積もり＞                    ⇨＜無効な注文＞
// 注文の各状態に対してモデリング
// 良くない例
// type Order = {
//   IsValidated: bool     // 検証時に設定される
//   IsPriced: bool        // 価格計算時に設定される
//   AmountToBill: decimal // 価格計算時に設定される
// }

// 良くない部分
// ・プログラマー側がこれらのOrderの状態を毎回チェックして扱う必要があり、ミスの原因になる
// ・プログラマー側が状態を担保すると言うことはコンパイラーは何も担保してくれない
// ・このモデリングから上記の注文の状態に関するドメイン情報を読み取れない

// 未検証の注文明細行
type UnvalidatedOrderLine = {
    LineId : string
}

// 検証済みの注文明細行
type ValidatedOrderLine = {
    LineId: string
}

// 未検証の注文書
type UnvalidatedOrder = {
    OrderId: string
    CustomerInfo: CustomerInfo
    ShippingAddress: Address
    BillingAddress: Address
    OrderLines: UnvalidatedOrderLine list
}

// 

// 検証済みの注文
type ValidatedOrder = {
    OrderId: string
    CustomerInfo: CustomerInfo
    ShippingAddress: Address
    BillingAddress: Address
    OrderLines: ValidatedOrderLine list
}

type PricedOrder = {
    OrderId: string
    CustomerInfo: CustomerInfo
}

// 注文をモデリング
type Order = 
    | Unvalidated of UnvalidatedOrder
    | Validated of ValidatedOrder
    | Priced of PricedOrder
    // 返金済みの注文がついkされたら以下のように定義すればいい
//  | Refunded of RefundedOrder

// ショッピングカートを例にステートマシンを実装する
type Item = {
    Id: string
    ItemName: string
    Price: int
}
type ActiveCartData = {
    UnpaidItems: Item list
}
type PaidCartData = {
    PaidItems: Item list
    Payment: float
}
type ShoppingCart = 
    | EmptyCart // データ無し
    | ActiveCart of ActiveCartData // アクティブなカート
    | PaidCart of PaidCartData // 支払い済みのカート

// 商品を追加するコマンドハンドラーを定義
// これは状態遷移関数
let addItem cart item = 
    match cart with
    | EmptyCart ->
        ActiveCart {
            UnpaidItems=[item]
        }
    | ActiveCart {UnpaidItems=existingItems}->
        ActiveCart {UnpaidItems = item :: existingItems}
    | PaidCart _ ->
        // 無視
        cart

// 商品を削除するコマンドハンドラーを定義
// ロバストにやるなら無視はしないで適切なエラーを返すべき
// 状態遷移関数
let deleteItem cart itemToDelete = 
    match cart with
    | EmptyCart ->
        // 無視
        cart
    | ActiveCart {UnpaidItems=existingItems} ->
        // リストから指定された商品を除く
        let newList =  existingItems |> List.filter (fun item -> item <> itemToDelete)
        
        if List.isEmpty newList then
            EmptyCart // 状態をEmptyCartに遷移させる
        else
            ActiveCart {UnpaidItems=newList}
        // 商品を削除する
    | PaidCart _ ->
        // 無視
        cart

// 支払い済みの状態にするための状態遷移関数
let makePayment cart payment =
    match cart with
    | EmptyCart ->
        // 無視
        cart
    | ActiveCart {UnpaidItems=existingItems} ->
        // 指定された支払いで、新しい支払い済みのカートを作成
        PaidCart {PaidItems = existingItems; Payment=payment}
    | PaidCart _ ->
        cart

// 型を使ったワークフローの各ステップのモデリング
// 検証のステップ
// 製品コードの存在チェック
type ProductCode = ProductCode of string
type checkProductCodeExists = ProductCode -> bool

// 検証済みのアドレス
type CheckedAddress = CheckedAddress of UnvalidatedAddress
// 住所の検証エラー
type AddressValidationError = AddressValidationError of string
// 注文の検証ステップ
type CheckAddressExists = UnvalidatedAddress -> Result<CheckedAddress, AddressValidationError>

// 注文の検証ステップ
type ValidationError = ValidationError of string
type ValidateOrder =
    checkProductCodeExists // 依存関係
        -> CheckAddressExists // 依存関係
        -> UnvalidatedOrder // 入力
        -> Result<ValidatedOrder, ValidationError>

// 価格設定のステップ
// 製品価格の取得
type Price = Price of int
type GetProductPrice =
    ProductCode -> Price
type PriceOrder =
    GetProductPrice // 依存関係
        -> ValidatedOrder // 入力
        -> PricedOrder // 出力

// 注文確認ステップ
type HtmlString = HtmlString of string
type EmailAddress = EmailAddress of string
// 注文確認
type OrderAcknowledgement = {
    EmailAddress : EmailAddress
    Letter : HtmlString
}
// 注文書の作成
type CreateOrderAcknowdgementLetter = PricedOrder -> HtmlString
type OrderAcknowledgementSent = {
    OrderId: int
    EmailAddress: EmailAddress
}
// 注文書の送信
// これは副作用のある関数
// 送信したイベント
type SendResult = Sent | NotSent
type SendOrderAcknowledgement = OrderAcknowledgement -> SendResult


type AckknowledgeOrder =
    CreateOrderAcknowdgementLetter // 依存関係
        -> SendOrderAcknowledgement // 依存関係
        -> PricedOrder // 入力
        -> OrderAcknowledgementSent // 出力

// 返すイベント
// 注文が確定したイベント
type OrderPlaced = PricedOrder
// 請求可能な注文が確定したイベント
type BillableOrderPlaced = {
    OrderId: string
    BillingAddress: Address
    AmountToBill: int
}

type PlaceOrderResult = {
    OrderPlaced: OrderPlaced
    BillableOrderPlaed: BillableOrderPlaced
    OrderAcknowledgementSent: OrderAcknowledgementSent option
}

// 注文確定イベント
type PlaceOrderEvent =
    | OrderPlaced of OrderPlaced
    | BillableOrderPlaced of BillableOrderPlaced
    | AckknowledgeSent of OrderAcknowledgementSent

type CreateEvents = PricedOrder -> PlaceOrderEvent list



printfn "--- カートにアイテムを追加します ---"
let item1 = {Id="id1";ItemName="シャツ";Price=8000}
let item2 = {Id="id2";ItemName="ズボン";Price=8000}
let cart1 = addItem EmptyCart item1
printfn "カート1の中身は¥n"
printfn "%A¥n" cart1

let cart2 = addItem cart1 item2

printfn "¥nカート2の中身は¥n"
printfn "¥n%A¥n" cart2

printfn "アイテム2を削除"
let deleteItemCart1 = deleteItem cart2 item2
printfn "¥n%A¥n" deleteItemCart1

printfn "アイテム1を削除"

let deleteItemCart2 = deleteItem deleteItemCart1 item1
printfn "¥n%A¥n" deleteItemCart2
