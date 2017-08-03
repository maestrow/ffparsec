namespace Parsec.Utils.Trees

type Tree<'i> = ResizeArray<Node<'i>>

and Node<'i> = {
  Item: 'i
  Children: Tree<'i>
}

[<AutoOpen>]
module Helpers = 

  let toNode<'i> (item: 'i) = { Node.Item = item; Children = new Tree<'i>() }


module Tree = 
  let map f tree : Tree<_> = 
    let rec inner tree = tree |> Seq.map mapNode |> Tree<_>
    and mapNode node =
      { 
        Item = f node.Item
        Children = inner node.Children
      }
    inner tree

  let toFlatList tree =
    let rec inner t level = t |> Seq.collect (fun n -> Seq.append [level, n.Item] (inner n.Children (level + 1)))
    inner tree 0