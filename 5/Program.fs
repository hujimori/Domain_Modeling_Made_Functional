// 複雑なデータのモデリング

// 未知の型のモデリング
// 設計段階でよくわからないモデルは以下のように定義できる
// 例外型のexnを使い、Undefined＜未定義＞とエイリアスする
type Undefined = exn

type CustonmerInfo = Undefined
type ShippingAddress = Undefined
type BillingAddress = Undefined
type OrderLine = Undefined
type BillingAmount = Undefined

type Order =
    { CustonmerInfo: CustonmerInfo
      ShippingAddress: ShippingAddress
      BillingAddress: BillingAddress
      OrderLines: OrderLine list
      AmountToBill: BillingAmount }

// undefinedで定義しているものは後から書き換えれば良い


// 選択型によるモデリング
type WidgetCode = Undefined
type GizmoCode = Undefined

type ProductCode =
    | Widget of WidgetCode
    | Gizomo of GizmoCode

type UnitOfQuantity = Undefined
type KilogramQuantity = Undefined

type OrderQuantity =
    | Unit of UnitOfQuantity
    | Kilogram of KilogramQuantity

// 関数によるワークフローのモデリング
type UnvalidatedOrder = Undefined
type ValidatedOrder = Undefined
type ValidateOrder = UnvalidatedOrder -> ValidatedOrder

// 複雑な入力と出力の処理
// 1つの入力に対して複数の入力があるワークフローの出力定義
// 複合型で表現する
type AcknowledgementSend = Undefined
type OrderPlaced = Undefined
type BillabledOrderPlaced = Undefined

type PlaceOrderEvents =
    { AcknowledgementSent: AcknowledgementSend
      OrderPlaced: OrderPlaced
      BillabledOrderPlaced: BillabledOrderPlaced }

// 注文確定のワークフローは以下のように定義できる
type PlaceOrder = UnvalidatedOrder -> PlaceOrderEvents
