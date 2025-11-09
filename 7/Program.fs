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