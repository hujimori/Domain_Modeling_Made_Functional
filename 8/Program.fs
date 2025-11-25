// F#で関数を「もの」として扱う
// let plus3 x = x + 3
// let time2 x = x * 2

// ２つのsquareの定義は本質的に同じ
// let square = (fun x -> x * x)
// let square x = x * x
// let addThree = plus3

// 関数をリストとしてまとめる
// let listOfFunctions = [ addThree; time2; square ]

// リストをループして、各関数を順番に評価する
// for fn in listOfFunctions do
//     let result = fn 100 // 関数を呼び出す
//     printfn "If 100 is the input, the output is %i" result

// 入力としての関数
// let evalWith5ThenAdd2 fn = fn (5) + 2

// let add1 x = x + 1
// let result = evalWith5ThenAdd2 add1
// printfn "evalWith5ThenAdd2 output is %i" result

// 出力としての関数
// let addGenerator numberToAdd =
//     // ラムダを返す
//     fun x -> numberToAdd + x

// 入力「x」に対して「1」を加算する関数を生成
// let add1 = addGenerator 1
// let add1result = add1 2
// printfn "add1result is %i" add1result
// 入力「x」に対して「100」を加算する関数を生成
// let add100 = addGenerator 100
// let add100result = add100 2
// printfn "add100result is %i" add100result

// カリー化
// 多パラメーターの関数でも、1パラメーターの関数を連なったものに変換する
// let add x y = x + y

// int -> (int -> int)
// let addGenerator x = fun y -> x + y

// 部分適用
let sayGreeting greeting name = printfn "%s %s" greeting name

// greetingパラメータを1つだけ渡して、新しい関数を作る
let sayHello = sayGreeting "Hello"

let sayGoodbye = sayGreeting "Goodbye"

sayHello "Alex"
sayGoodbye "Alex"

// 全域関数
type NonZeroInteger =
    // ゼロではない整数に制約されるように定義する
    // スマートコンストラクタを追加するなど
    private | NonZeroInteger of int

module NonZeroInteger =
    let create n =
        if n = 0 then
            Error "0は許可されていません（ゼロ除算）になります"
        else
            Ok(NonZeroInteger n)

    let value (NonZeroInteger n) = n

let twelveDividedBy (input) =
    let n = NonZeroInteger.value input

    12 / n

let invalidInput = NonZeroInteger.create 0

match invalidInput with
| Ok value -> printfn "計算結果: %d" (twelveDividedBy value)
| Error msg -> printfn "エラー：%s" msg

let validInput = NonZeroInteger.create 4

match validInput with
| Ok value -> printfn "計算結果：%d" (twelveDividedBy value)
| Error msg -> printfn "エラー：%s" msg

// F#における関数の合成
let add1 x = x + 1
let square x = x * x

let add1ThenSquare x = x |> add1 |> square

// テスト
let result = add1ThenSquare 5
printfn "%d" result

let isEven x = x % 2 = 0

let printBool x = sprintf "value is %b" x

let isEvenThenPrint x = x |> isEven |> printBool

let x = isEvenThenPrint 2
printfn "%s" x

// 関数を合成する上での課題
// 一方の関数の出力がもう一方の関数の入力と不一致の場合はどうするか
// アプローチの一つとして、両サイドを同じ型、両サイドの「最小公倍数」に変換すること

// intを出力とする関数
let addPlus1 x = x + 1

// Option<int>を入力とする関数
let printOption x =
    match x with
    | Some i -> printfn "The int is %i" i
    | None -> printfn "No Value"

5 |> addPlus1 |> Some |> printOption
