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
// type ValidateOrder = UnvalidatedOrder -> ValidatedOrder

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

// ワークフローがouputAかoutputBのどちらかを出力する場合
// どちらでも格納できる選択型を作成する
type EnvelopContents = EnvelopContents of string

type QuoteForm = Undefined
type OrderForm = Undefined

type CategorizedMail =
    | Quote of QuoteForm // 見積もり依頼
    | Order of OrderForm // 注文依頼

type CategorizeInboundMail = EnvelopContents -> CategorizedMail

// ワークフローに異なる入力の選択肢がある場合
// 最もシンプルな方法
// type ProductCatalog = Undefined
// type PricedOrder = Undefined
// type CalculatePrices = OrderForm -> ProductCatalog -> PricedOrder

// 両方を含む新しいレコード型のように定義できる
type ProductCatalog = Undefined
type PricedOrder = Undefined

type CalculatePricesInput =
    { OrderForm: OrderForm
      ProductCatalog: ProductCatalog }

type CalculatePrices = CalculatePricesInput -> PricedOrder

// 関数のシグネチャでエフェクトを文書化する
// 検証が常に成功しないことを表現する
// type ValidateOrder = UnvalidatedOrder -> Result<ValidatedOrder, ValidationError list>

// 非同期エフェクトを表す
//type ValidateOrder = UnvalidatedOrder -> Async<Result<ValidatedOrder, ValidationError list>>

// 型のエイリアスを作成してシンプルに書く
type ValidationError =
    { FieldName: string
      ErrorDescription: string }

type ValidationResponse<'a> = Async<Result<'a, ValidationError list>>
type ValidateOrder = UnvalidatedOrder -> ValidationResponse<ValidatedOrder>
