using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;

namespace ID3
{
    class TreeNode
    {
        private ArrayList mChilds = null;
        private Attribute mAttribute;

        
        public TreeNode(Attribute attribute)
        {
            if (attribute.values != null)
            {
                mChilds = new ArrayList(attribute.values.Length);
                for (int i = 0; i < attribute.values.Length; i++)
                    mChilds.Add(null);
            }
            else
            {
                mChilds = new ArrayList(1);
                mChilds.Add(null);
            }
            mAttribute = attribute;
        }
        public void AddTreeNode(TreeNode treeNode, string ValueName)
        {
            int index = mAttribute.indexValue(ValueName);
            mChilds[index] = treeNode;
        }

        public int totalChilds
        {
            get
            {
                return mChilds.Count;
            }
        }
        public TreeNode getChild(int index)
        {
            return (TreeNode)mChilds[index];
        }
        public Attribute attribute
        {
            get
            {
                return mAttribute;
            }
        }
        public TreeNode getChildByBranchName(string branchName)
        {
            int index = mAttribute.indexValue(branchName);
            return (TreeNode)mChilds[index];
        }
    }
        
}
