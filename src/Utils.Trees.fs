namespace Parsec.Utils.Trees

type Tree<'i> = ResizeArray<Node<'i>>

and Node<'i> = {
  Item: 'i
  Children: Tree<'i>
}

[<AutoOpen>]
module Helpers = 

  let toNode<'i> (item: 'i) = { Node.Item = item; Children = new Tree<'i>() }