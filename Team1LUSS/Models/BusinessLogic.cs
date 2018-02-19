using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Net.Mail;
using System.Data.Entity.Validation;
using System.Collections;

namespace Team1LUSS.Models
{
    public class BusinessLogic
    {
        LUSSISEntities ls = new LUSSISEntities();
        //Reuben - for catalogue page
        public List<Catalogue> GetCatalogue
        {
            get
            {
                return ls.Catalogues.ToList<Catalogue>();
            }
        }
        //Reuben - for DeptHeadMainReq
        public List<Requisition> GetPendingReqs(string dept)
        {
            return ls.Requisitions.Where(p => p.Status == "Pending" && p.Employee1.DepartmentID == dept).ToList();
        }
        //Reuben - for DeptHeadMainReq
        public List<Requisition> GetAllOtherReqs(string dept)
        {
            return ls.Requisitions.Where(p => p.Status != "Pending" && p.Employee1.DepartmentID == dept).ToList();
        }
        //Reuben - for requisitions: pending and details pages
        public List<RequisitionDetail> GetRequisitionDetails(int id)
        {
            List<RequisitionDetail> reqdetails = new List<RequisitionDetail>();
            return ls.RequisitionDetails.Where(m => m.RequisitionID == id).ToList();
        }

        //Reuben - to add items from approved Req Form to Interim Stock Retreival Table.
        public static void UpdateInterimQtyFromReq(int id)
        {
            LUSSISEntities ls = new LUSSISEntities();

            List<RequisitionDetail> rdetails = new List<RequisitionDetail>();
            rdetails = ls.RequisitionDetails.Where(m => m.RequisitionID == id).ToList();
            foreach (RequisitionDetail r in rdetails)
            {
                using (System.Transactions.TransactionScope ts = new TransactionScope())
                {
                    InterimRetrievalStockTable i = ls.InterimRetrievalStockTables.Where(x => x.ItemCode == r.ItemCode).First();
                    i.WaitingQty += r.ItemQty;
                    ls.SaveChanges();
                    ts.Complete();
                }
            }
        }

        //Reuben - for stock card
        public List<StockCard> GetStockCard(string itemcode)
        {
            List<StockCard> scard = new List<StockCard>();
            return ls.StockCards.Where(m => m.ItemCode == itemcode).ToList();
        }
        //Reuben - for pending page, to update the requisition status
        public static void UpdateRequsitionByID(int id, int aid, DateTime now, string status, string comment)
        {
            LUSSISEntities e = new LUSSISEntities();

            using (TransactionScope ts = new TransactionScope())
            {
                Requisition r = e.Requisitions.Where(x => x.RequisitionID == id).First();
                //do update 
                r.AuthorizedBy = aid;
                r.AuthorizedDate = now;
                r.Status = status;
                r.Comment = comment;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Reuben - for stock card to list suppliers of that product
        public List<ItemSupplier> GetSuppliersByItem(string itemcode)
        {
            List<ItemSupplier> supplist = new List<ItemSupplier>();
            return ls.ItemSuppliers.Where(m => m.ItemCode == itemcode).ToList();
        }
        //Reuben - for supplier list page
        public List<Supplier> GetSuppliers
        {
            get
            {
                return ls.Suppliers.ToList<Supplier>();
            }
        }
        //Reuben - for supplier card
        public List<ItemSupplier> GetAItemsBySupplier(string suppID)
        {
            List<ItemSupplier> alist = new List<ItemSupplier>();
            return ls.ItemSuppliers.Where(m => m.SupplierID == suppID && m.RankofSupplierforItem == "A").ToList();
        }
        //Reuben - for supplier card
        public List<ItemSupplier> GetBItemsBySupplier(string suppID)
        {
            List<ItemSupplier> blist = new List<ItemSupplier>();
            return ls.ItemSuppliers.Where(m => m.SupplierID == suppID && m.RankofSupplierforItem == "B").ToList();
        }
        //Reuben - for supplier card
        public List<ItemSupplier> GetCItemsBySupplier(string suppID)
        {
            List<ItemSupplier> clist = new List<ItemSupplier>();
            return ls.ItemSuppliers.Where(m => m.SupplierID == suppID && m.RankofSupplierforItem == "C").ToList();
        }
        //Reuben - for supplier card
        public List<ItemSupplier> GetAllItemsBySupplier(string suppID)
        {
            List<ItemSupplier> allitemslist = new List<ItemSupplier>();
            return ls.ItemSuppliers.Where(m => m.SupplierID == suppID).ToList();
        }
        //Reuben - Used for Login page to get the user
        public Employee GetEmployeeByID(int id)
        {
            List<Employee> employee = ls.Employees.Where(c => c.EmployeeID == id).ToList<Employee>();
            if (employee.Count > 0)
                return employee[0];
            else
                return null;
        }
        //Reuben 
        public static void UpdateCollectionPoint(string id, string point)
        {
            LUSSISEntities e = new LUSSISEntities();

            using (TransactionScope ts = new TransactionScope())
            {
                Department d = e.Departments.Where(x => x.DepartmentID == id).First();
                //do update 
                d.CollectionPoint = point;
                e.SaveChanges();
                ts.Complete();
            }
        }

        //Sandor
        public static List<Requisition> getReqList()
        {
            LUSSISEntities context = new Models.LUSSISEntities();
            List<Requisition> Reqlist = new List<Requisition>();

            Reqlist = context.Requisitions.ToList();

            return Reqlist;
        }
        //Sandor
        public static void deleteReqList(int Reqid)
        {
            LUSSISEntities context = new LUSSISEntities();

            using (TransactionScope scope = new TransactionScope())
            {
                Requisition req = context.Requisitions.Where(x => x.RequisitionID == Reqid).First();

                context.Requisitions.Remove(req);
                context.SaveChanges();

                scope.Complete();
            }
        }
        //Sandor
        public static void deleteReqListDetail(int Reqid)
        {
            LUSSISEntities context = new LUSSISEntities();

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<RequisitionDetail> reqlist = context.RequisitionDetails.Where(x => x.RequisitionID == Reqid).ToList();
                    foreach (var i in reqlist)
                    {
                        context.RequisitionDetails.Remove(i);
                        context.SaveChanges();
                    }
                    scope.Complete();
                }
            }
            catch (Exception e) { }
        }
        //Sandor
        public static List<RequisitionDetail> getReqDetailByID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            List<RequisitionDetail> rlist = e.RequisitionDetails.Where(x => x.RequisitionID == id).ToList();
            return rlist;
        }
        //Sandor
        public static Requisition getRequisitionByID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Requisition r = e.Requisitions.Where(x => x.RequisitionID == id).First();
            return r;
        }
        //Sandor
        public static List<Catalogue> listAllCatelogue()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Catalogue> clist = e.Catalogues.ToList();
            return clist;
        }
        //Sandor
        public static Catalogue listSelectedItem(String ic)
        {

            LUSSISEntities e = new LUSSISEntities();
            Catalogue item = e.Catalogues.Where(x => x.ItemCode == ic).First();

            return (item);
        }
        //Sandor
        public static void createRequisition(int employeeID)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                LUSSISEntities e = new LUSSISEntities();

                Requisition req = new Requisition();
                req.EmployeeID = employeeID;
                req.Date = DateTime.Now;
                req.Status = "Pending";

                e.Requisitions.Add(req);
                e.SaveChanges();

                scope.Complete();
            }
        }
        //Sandor
        public static void createRequisitionDetail(int requisitionID, String itemcode, int itemqty)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                LUSSISEntities e = new LUSSISEntities();

                RequisitionDetail reqd = new RequisitionDetail();
                reqd.RequisitionID = requisitionID;
                reqd.ItemCode = itemcode;
                reqd.ItemQty = itemqty;
                reqd.OutstandingQty = itemqty;
                reqd.DisbursedQty = 0;

                e.RequisitionDetails.Add(reqd);
                e.SaveChanges();

                scope.Complete();
            }
        }
        //Sandor
        public static List<Requisition> getRequisitionByEmployeeID(int empid)
        {
            LUSSISEntities e = new LUSSISEntities();

            List<Requisition> depreqlist = new List<Requisition>();
            List<Employee> emplist = getEmpInOneDep(empid);

            foreach (Employee emp in emplist)
            {
                int eid = emp.EmployeeID;
                depreqlist.AddRange(e.Requisitions.Where(x => x.EmployeeID == eid).ToList());
            }
            return depreqlist;
        }

        //Sandor
        public static List<Employee> getEmpInOneDep(int empid)
        {
            LUSSISEntities e = new LUSSISEntities();

            string depid = e.Employees.Where(x => x.EmployeeID == empid).Select(x => x.DepartmentID).First();
            List<Employee> emplist = e.Employees.Where(x => x.DepartmentID == depid).ToList();

            return emplist;

        }
        //Sandor
        public static int getBottomRequisitionID()
        {
            LUSSISEntities e = new LUSSISEntities();

            //int ID = e.Requisitions.Where(x=>).Select(x=>x.RequisitionID).SingleOrDefault;
            var rev = (from x in e.Requisitions
                       orderby x.RequisitionID descending
                       select x.RequisitionID).First();

            int id = Convert.ToInt16(rev);
            return id;

        }
        //Sandor
        public static void updateReqDetail(int reqid, string itemcode, int qty)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                LUSSISEntities e = new LUSSISEntities();

                //int[] Qty = Convert.ToInt16(string[]qty);
                //for(int i=0;i<itemcode.Length;i++)

                RequisitionDetail reqd = e.RequisitionDetails.Where(x => x.RequisitionID == reqid && x.ItemCode == itemcode).First();
                reqd.ItemQty = qty;
                reqd.OutstandingQty = qty;
                e.SaveChanges();

                scope.Complete();
            }

        }
        //Irene
        public List<Requisition> GetAllApprovedReqs()
        {
            LUSSISEntities e = new LUSSISEntities();
            return e.Requisitions.Where(p => p.Status == "Approved").ToList();
        }

        //Luowei
        public static List<PurchaseOrderDetail> listorderdetails(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            List<PurchaseOrderDetail> lpod = e.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == id).ToList();
            //List<PurchaseOrderDetail> lpod = e.PurchaseOrderDetails.Where(x=>x.PurchaseOrderID==id).ToList();
            return lpod;
        }
        //Luowei
        public static List<Supplier> getSupplierlist()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Supplier> slist = e.Suppliers.ToList();
            return slist;
        }
        //Luowei
        public static List<PurchaseOrder> listorder()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<PurchaseOrder> lo = e.PurchaseOrders.Where(x => x.Status != "Pending").ToList();
            return lo;
        }
        //Luowei
        public static List<PurchaseOrder> listpendingorder()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<PurchaseOrder> lpo = e.PurchaseOrders.Where(x => x.Status == "Pending").ToList();
            return lpo;
        }
        //Luowei
        public static List<Employee> getEmployeelistByDep(String id)
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Employee> elist = e.Employees.Where(x => x.Department.DepartmentID == id).ToList();
            return elist;
        }
        //Luowei
        public static PurchaseOrder getPurchaseOrderID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            PurchaseOrder po = e.PurchaseOrders.Where(x => x.PurchaseOrderID == id).First();
            return po;
        }
        //Luowei
        public static Employee getActingHead(String id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Employee em = e.Employees.Where(x => x.Authorization == "Authorized" && x.DepartmentID == id).First();
            return em;
        }
        //Luowei
        public static void setActingNullByID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Employee em = e.Employees.Where(x => x.EmployeeID == id).First();
            using (TransactionScope ts = new TransactionScope())
            {
                em.Authorization = null;
                //em.StartDate = null;
                //em.EndDate = null;
                e.SaveChanges();
                ts.Complete();
            }

        }
        //Luowei
        public static void setActingHeadByID(int id, DateTime st, DateTime ed)
        {
            LUSSISEntities e = new LUSSISEntities();

            using (TransactionScope ts = new TransactionScope())
            {
                Employee em = e.Employees.Where(x => x.EmployeeID == id).First();
                em.Authorization = "Authorized";
                em.StartDate = st;
                em.EndDate = ed;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Luowei
        public static void setHeadBackByID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();

            using (TransactionScope ts = new TransactionScope())
            {
                Employee em = e.Employees.Where(x => x.EmployeeID == id).First();
                em.Authorization = "Authorized";
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Luowei
        public static void setApprovedPO(int pid, int eid)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                PurchaseOrder po = e.PurchaseOrders.Where(x => x.PurchaseOrderID == pid).First();
                po.Status = "Approved";
                po.ApprovalBy = eid;
                po.ApprovalDate = DateTime.Now;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Luowei
        public static void setRejectedPO(int pid, int eid)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                PurchaseOrder po = e.PurchaseOrders.Where(x => x.PurchaseOrderID == pid).First();
                po.Status = "Rejected";
                po.ApprovalBy = eid;
                po.ApprovalDate = DateTime.Now;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Luowei
        public static Employee getCurrentUserByUserName(string username)
        {
            LUSSISEntities e = new LUSSISEntities();
            Employee em = e.Employees.Where(x => x.EmployeeName == username).First();
            return em;
        }
        //Luowei
        public static Supplier getSupplierByID(string id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Supplier s = e.Suppliers.Where(x => x.SupplierID == id).First();
            return s;
        }
        //Luowei
        public static List<Supplier> GetSuplierListInPO()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<String> slist = e.PurchaseOrderDetails.Select(x => x.Supplier).Distinct().ToList();
            List<Supplier> sulist = new List<Supplier>();
            Supplier s;
            foreach (var sitem in slist)
            {
                s = BusinessLogic.getSupplierByID(sitem);
                sulist.Add(s);
            }
            return sulist;
        }
        //Luowei
        public static List<Supplier> GetSuplierListInPOByID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            List<String> slist = e.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == id).Select(x => x.Supplier).Distinct().ToList();
            List<Supplier> sulist = new List<Supplier>();
            Supplier s;
            foreach (var sitem in slist)
            {
                s = BusinessLogic.getSupplierByID(sitem);
                sulist.Add(s);
            }
            return sulist;
        }
        //Reuben - Added to LW's Approve() - to add transactions to stock card & update catalogue Qty.
        public static void UpdateStockCardAndCatalogue(int pid)
        {
            LUSSISEntities ls = new LUSSISEntities();

            List<PurchaseOrderDetail> pdetails = new List<PurchaseOrderDetail>();
            pdetails = ls.PurchaseOrderDetails.Where(m => m.PurchaseOrderID == pid).ToList();
            foreach (PurchaseOrderDetail p in pdetails)
            {
                List<StockCard> stranlist = new List<StockCard>();
                stranlist = ls.StockCards.Where(x => x.ItemCode == p.ItemCode).ToList();
                int bal = (int)stranlist.Last().Balance;

                using (TransactionScope ts = new TransactionScope())
                {
                    StockCard s = new StockCard();
                    s.ItemCode = p.ItemCode;
                    s.TransactionDate = DateTime.Now.Date;
                    s.Dept_Supplier_Adjt = "Supplier-" + p.Supplier;
                    s.Qty = p.Qty;
                    s.AddorMinus = "+";
                    s.Balance = bal + p.Qty;
                    ls.StockCards.Add(s);
                    ls.SaveChanges();
                    ts.Complete();
                }
                using (TransactionScope td = new TransactionScope())
                {
                    Catalogue c = ls.Catalogues.Where(x => x.ItemCode == p.ItemCode).First();
                    c.CurrentStockQty += p.Qty;
                    ls.SaveChanges();
                    td.Complete();
                }
            }
        }        

        //Claire        
        //public int getCount()
        //{
        //    return findAllAdjustment().Count;
        //}

        //Claire
        public static Catalogue FindItemByItemID(string id)
        {
            LUSSISEntities m = new LUSSISEntities();
            Catalogue e = m.Catalogues.Where(x => x.ItemCode == id).First();
            return e;
        }
        //Claire
        public static List<AdjustmentVoucher> FindAllAdjustment()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = m.AdjustmentVouchers.ToList();
            return list;
        }
        //Claire
        public static List<AdjustmentVoucher> FindAllAdjustmentPendingClerk()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = m.AdjustmentVouchers.Where(x => x.Status == "Pending").ToList();//whether system created and certain clerk
            return list;
        }
        //Claire
        public static List<AdjustmentVoucher> FindAllAdjustmentComplete()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = m.AdjustmentVouchers.Where(x => x.Status != "Pending").ToList();
            return list;
        }

        //Claire
        public static List<AdjustmentVoucher> FindAllAdjustmentForManagerPending()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = m.AdjustmentVouchers.Where(x => x.Qty * x.Catalogue.AveragePrice >= 250).Where(x => x.Status == "Pending").ToList();
            return list;
        }
        //Claire
        public static List<AdjustmentVoucher> FindAllAdjustmentForManagerComplete()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = m.AdjustmentVouchers.Where(x => x.Qty * x.Catalogue.AveragePrice >= 250).Where(x => x.Status != "Pending").ToList();
            return list;
        }
        //Claire
        public static List<AdjustmentVoucher> FindAllAdjustmentForSupervisorPending()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = m.AdjustmentVouchers.Where(x => x.Qty * x.Catalogue.AveragePrice < 250).Where(x => x.Status == "Pending").ToList();
            return list;
        }
        //Claire
        public static List<AdjustmentVoucher> FindAllAdjustmentForSupervisorComplete()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = m.AdjustmentVouchers.Where(x => x.Qty * x.Catalogue.AveragePrice < 250).Where(x => x.Status != "Pending").ToList();
            return list;
        }
        //Claire
        public static string[] FindApproveEmployees()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<AdjustmentVoucher> list = BusinessLogic.FindAllAdjustmentComplete();
            int s = list.Count;
            string[] Approvalname = new string[s];

            for (int i = 0; i < list.Count; i++)
            {

                if (list[i].ApprovalBy == null)
                {
                    Approvalname[i] = null;
                }
                else
                {
                    int id = Convert.ToInt16(list[i].ApprovalBy);
                    Employee e = m.Employees.Where(x => x.EmployeeID == id).First();
                    Approvalname[i] = e.EmployeeName;
                }

            }
            return Approvalname;

        }

        //Claire
        public static void SetAdjustmentToRejectByID(int id, int userid)
        {
            using (LUSSISEntities m = new LUSSISEntities())
            {
                AdjustmentVoucher a = m.AdjustmentVouchers.Where(x => x.AdjustmentVoucherID == id).First();
                a.Status = "Rejected";
                a.ApprovalBy = userid;
                // a.ApprovalBy = "";(currentuser)
                a.ApprovalDate = DateTime.Now;
                m.SaveChanges();
            }
        }
        //Claire
        public static void SetAdjustmentToAgreeByID(int id, int userid)
        {
            using (LUSSISEntities m = new LUSSISEntities())
            {
                AdjustmentVoucher a = m.AdjustmentVouchers.Where(x => x.AdjustmentVoucherID == id).First();
                a.Status = "Approved";
                a.ApprovalBy = userid;
                // a.ApprovalBy = "";(currentuser)
                a.ApprovalDate = DateTime.Now;
                m.SaveChanges();
            }
        }
        //Claire
        public static AdjustmentVoucher FindAdjustmentByID(int id)
        {
            LUSSISEntities m = new LUSSISEntities();
            AdjustmentVoucher a = m.AdjustmentVouchers.Where(x => x.AdjustmentVoucherID == id).First();
            return a;
        }
        //Claire
        public static void UpdateAdjustmentByID(int id, string addorminus, int quantity, string reason)
        {
            using (LUSSISEntities m = new LUSSISEntities())
            {
                AdjustmentVoucher a = m.AdjustmentVouchers.Where(x => x.AdjustmentVoucherID == id).First();
                a.AddorMinus = addorminus;
                a.Qty = quantity;
                a.Reason = reason;
                a.Amount = quantity * a.Catalogue.AveragePrice;
                a.Reason = reason;
                m.SaveChanges();
            }


        }
        //Claire
        public static List<Catalogue> FindAllCatalogue()
        {
            LUSSISEntities m = new LUSSISEntities();
            List<Catalogue> list = m.Catalogues.ToList();
            return list;

        }
        //public static List<Category> FindAllCategory()
        //{
        //    LUSSISEntities m = new LUSSISEntities();
        //    List<Category> list = m.Categories.ToList();
        //    return list;
        //}
        //Claire
        public static List<Catalogue> FindCatalogueByCatagoryid(int id)
        {
            LUSSISEntities m = new LUSSISEntities();
            List<Catalogue> list = m.Catalogues.Where(x => x.CategoryID == id).ToList();
            return list;
        }
        //Claire
        public static void DeleteAdjustmentByID(int id)
        {
            using (LUSSISEntities m = new LUSSISEntities())
            {
                AdjustmentVoucher a = m.AdjustmentVouchers.Where(x => x.AdjustmentVoucherID == id).First();
                m.AdjustmentVouchers.Remove(a);
                m.SaveChanges();
            }


        }
        //Claire
        public static Department FindCurrentRepresentative(string id)
        {
            LUSSISEntities m = new LUSSISEntities();

            Department de = m.Departments.Where(x => x.DepartmentID == id).First();
            return de;

        }
        //Claire
        public static void CreateAdjustment(List<AdjustmentVoucher> list)
        {
            LUSSISEntities m = new LUSSISEntities();
            foreach (var item in list)
            { 
                using (TransactionScope ts=new TransactionScope())
                {
                    Catalogue c = m.Catalogues.Where(x => x.ItemCode == item.ItemCode).First();
                    AdjustmentVoucher newone = new Models.AdjustmentVoucher
                    {// create person
                        ItemCode = item.ItemCode,
                        Date = item.Date,
                        AddorMinus = item.AddorMinus,
                        Qty = item.Qty,
                        Reason = item.Reason,
                        Amount = item.Qty * c.AveragePrice,
                        ClerkID = item.ClerkID,
                        Status = item.Status
                    };
                    m.AdjustmentVouchers.Add(newone);
                    m.SaveChanges();
                    ts.Complete();
                }
            }
        }
        //public static Employee getCurrentUserByUserName(string username)
        //{
        //    LUSSISEntities e = new Models.LUSSISEntities();
        //    Employee em = e.Employees.Where(x => x.EmployeeName == username).First();
        //    return em;
        //}
        //public static List<Employee> getEmployeelistByDep(string id)
        //{
        //    LUSSISEntities e = new Models.LUSSISEntities();
        //    List<Employee> elist = e.Employees.Where(x => x.Department.DepartmentID == id).ToList();
        //    return elist;
        //}

        //Claire
        public static void UpdateAutorityRep(int id)
        {
            LUSSISEntities m = new Models.LUSSISEntities();
            using (TransactionScope ts=new TransactionScope())
            {
                Employee e = m.Employees.Where(x => x.EmployeeID == id).First();
                Department d = m.Departments.Where(x => x.DepartmentID == e.DepartmentID).First();
                d.RepresentativeName = e.EmployeeName;
                m.SaveChanges();
                ts.Complete();
            }
        }
        //Claire
        public static void UpdateCurrentStockQty(int id)
        {
            LUSSISEntities m = new LUSSISEntities();
            using(TransactionScope ts=new TransactionScope()) {
            AdjustmentVoucher ad = m.AdjustmentVouchers.Where(x => x.AdjustmentVoucherID == id).First();
            Catalogue ca = m.Catalogues.Where(x => x.ItemCode == ad.ItemCode).First();
            if (ad.AddorMinus == "+")
            {
                ca.CurrentStockQty = ca.CurrentStockQty + ad.Qty;
            }
            else
            {
                ca.CurrentStockQty = ca.CurrentStockQty - ad.Qty;
            }m.SaveChanges();
                ts.Complete();
            }
        }
        //Claire
        public static void CreateNewStockCard(int id)
        {
            LUSSISEntities m = new LUSSISEntities();
            using (TransactionScope ts=new TransactionScope())
            {
                AdjustmentVoucher ad = m.AdjustmentVouchers.Where(x => x.AdjustmentVoucherID == id).First();
                Catalogue ca = m.Catalogues.Where(x => x.ItemCode == ad.ItemCode).First();
                if (ad.AddorMinus == "+")
                {
                    StockCard st = new StockCard
                    {
                        ItemCode = ad.ItemCode,
                        TransactionDate = ad.Date,
                        Dept_Supplier_Adjt = "Adjustment",
                        Qty = ad.Qty,
                        AddorMinus = ad.AddorMinus,
                        Balance = ca.CurrentStockQty + ad.Qty

                    };
                    m.StockCards.Add(st);
                    m.SaveChanges();
                }
                else
                {
                    StockCard st = new StockCard
                    {
                        ItemCode = ad.ItemCode,
                        TransactionDate = ad.Date,
                        Dept_Supplier_Adjt = "Adjustment",
                        Qty = ad.Qty,
                        AddorMinus = ad.AddorMinus,
                        Balance = ca.CurrentStockQty - ad.Qty

                    };
                    m.StockCards.Add(st);
                    m.SaveChanges();

                }
                ts.Complete();

            }
        }
        //Claire
        public static void UpdatePreviousRep(string name)
        {
            LUSSISEntities m = new LUSSISEntities();
            using(TransactionScope ts=new TransactionScope())
            {
                Employee e = m.Employees.Where(x => x.EmployeeName == name).First();
                e.Authorization = null;
                m.SaveChanges();
                ts.Complete();
            
            }
        }
        //Claire
        public static void UpdateWantToChangeRep(int id)
        {
            LUSSISEntities m = new LUSSISEntities();
            using(TransactionScope ts=new TransactionScope())
            {
                Employee e = m.Employees.Where(x => x.EmployeeID == id).First();
                e.Authorization = "Representative";
                m.SaveChanges();
                ts.Complete();

            }
        }
        //christine
        public static List<Department> showDepartmentList()
        {
            LUSSISEntities entity = new LUSSISEntities();
            List<Department> user = entity.Departments.ToList();
            return user;
        }
        //christine
        public static List<String> showDepartmentCode()
        {
            LUSSISEntities entity = new LUSSISEntities();
            List<String> olist = entity.Employees.Select(x => x.DepartmentID).ToList();
            return olist;
        }
        //christine
        public static List<Department> getDepartment()
        {
            LUSSISEntities entity = new LUSSISEntities();
            List<Department> olist = entity.Departments.ToList();
            return olist;
        }
        //christine
        public static List<Disbursement> GetDisbursment()
        {
            LUSSISEntities entity = new LUSSISEntities();
            List<Disbursement> olist = entity.Disbursements.ToList();
            return olist;
        }
        //christine
        public static List<Disbursement> GetDisbursmentCompleted()
        {
            LUSSISEntities entity = new LUSSISEntities();
            List<Disbursement> olist = entity.Disbursements.Where(x => x.Status == "Confirmed").ToList();
            return olist;
        }
        //christine
        public static List<Disbursement> GetDisbursmentCompletedBymonth(string d)
        {
            LUSSISEntities entity = new LUSSISEntities();
            DateTime d1 = Convert.ToDateTime("2018-" + d + "-1");            
            DateTime d2;
            if (Convert.ToInt32(d) == 2)
            {
                d2 = Convert.ToDateTime("2018-" + d + "-28");
            }
            else
            {
                d2 = Convert.ToDateTime("2018-" + d + "-30");
            }
            List<Disbursement> olist = entity.Disbursements.Where(x => x.Status == "Confirmed").Where(x => x.Disbursement_Date < d2).Where(x => x.Disbursement_Date > d1).ToList();
            return olist;
        }
        //christine
        public static List<Disbursement> GetDisbursmentCompletedByDepartment(string id)
        {
            LUSSISEntities entity = new LUSSISEntities();
            //List<Disbursement> olist = entity.Disbursements.Where(x => x.Employee1.DepartmentID == id).Where(x => x.Status == "Confirmed").ToList();
            List<Disbursement> olist = entity.Disbursements.Where(x => x.DepartmentID == id && x.Status == "Confirmed").ToList();
            return olist;
        }
        //christine
        public static List<Disbursement> GetDisbursmentCompletedByDepartmentBymonth(string id, string t)
        {
            LUSSISEntities entity = new LUSSISEntities();
            DateTime d1 = Convert.ToDateTime("2018-" + t + "-1");
            DateTime d2;
            if (Convert.ToInt32(t)==2)
            {
                d2 = Convert.ToDateTime("2018-" + t + "-28");
            }
            else
            {
                d2 = Convert.ToDateTime("2018-" + t + "-30");
            }
            
            List<Disbursement> olist = entity.Disbursements.Where(x => x.Employee1.DepartmentID == id).Where(x => x.Status == "Confirmed").Where(x => x.Disbursement_Date < d2).Where(x => x.Disbursement_Date > d1).ToList();
            //List<Disbursement> olist = entity.Disbursements.Where(x => x.DepartmentID == id && x.Status == "Confirmed" && x.Disbursement_Date < d2 && x.Disbursement_Date > d1).ToList();
            return olist;
        }

        //christine
        public static List<DisbursementDetail> GetDisbursmentDetailCompleted(int id)
        {
            LUSSISEntities entity = new LUSSISEntities();

            Disbursement d = entity.Disbursements.Where(x => x.DisbursementID == id && x.Status == "Confirmed").First();

            List<DisbursementDetail> olist = entity.DisbursementDetails.Where(x => x.DisbursementID == d.DisbursementID).ToList();
            return olist;
        }
        //Irene
        public static void UpdateStatus(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                Disbursement ditem = e.Disbursements.Where(x => x.DisbursementID == id).First();                
                ditem.Status = "Charge Back Completed";
                e.SaveChanges();
                ts.Complete();
            }
        }

        //Irene
        public static Employee GetDeptHeadByDeptID(String deptID)
        {
            LUSSISEntities e = new LUSSISEntities();
            Employee emp = e.Employees.Where(x => x.DepartmentID == deptID && x.RoleID == 2).First();
            return emp;
        }
        //Irene
        public static List<Disbursement> GetDisbursementListByDept(String deptid)
        {
            LUSSISEntities e = new LUSSISEntities();
            List <Disbursement> dlist = e.Disbursements.Where(x => x.DepartmentID == deptid && x.Status == "Confirmed").ToList();
            return dlist;
        }
        //Irene
        public static List<PurchaseOrder> listPO()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<PurchaseOrder> lp = e.PurchaseOrders.ToList();
            return lp;
        }
        //Irene
        public static List<Supplier> getSupplierActive()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Supplier> slist = e.Suppliers.Where(x => x.Status == "Active").ToList();
            return slist;
        }
        //Irene
        public static Catalogue getCatalogueByID(String id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Catalogue c = e.Catalogues.Where(x => x.ItemCode == id).First();
            return c;
        }
        //Irene
        public static List<PurchaseOrder> listapprovedorderbyEmployee(int empid)
        {
            LUSSISEntities e = new LUSSISEntities();
            List<PurchaseOrder> lpo = e.PurchaseOrders.Where(x => x.OrderBy == empid && x.Status == "Approved").ToList();
            return lpo;
        }
        //Irene
        public static List<PurchaseOrder> listpendingorderbyEmployee(int empid)
        {
            LUSSISEntities e = new LUSSISEntities();
            List<PurchaseOrder> lpo = e.PurchaseOrders.Where(x => x.OrderBy == empid && x.Status == "Pending").ToList();
            return lpo;
        }
        //Irene
        public static ItemSupplier getItemSupplierByID(String itemid, String rank)
        {
            LUSSISEntities e = new LUSSISEntities();
            ItemSupplier itemSupplier = e.ItemSuppliers.Where(x => x.ItemCode == itemid && x.RankofSupplierforItem == rank).First();
            return itemSupplier;
        }
        //Irene
        public static List<Catalogue> GetOrderCatalogue()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Catalogue> orderlist = e.Catalogues.Where(x => x.CurrentStockQty <= (1.1 * x.ReorderLevel)).ToList();
            return orderlist;
        }
        //Irene
        public static Employee getEmployeeByID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Employee e1 = e.Employees.Where(x => x.EmployeeID == id).First();
            return e1;
        }
        //Irene
        public static int AddItem(int empid)
        {
            LUSSISEntities e = new LUSSISEntities();
            LUSSISEntities e1 = new LUSSISEntities();
            Employee emp = e1.Employees.Where(x => x.EmployeeID == empid).First();
            using (TransactionScope ts = new TransactionScope())
            {
                PurchaseOrder po = new PurchaseOrder();
                po.Date = DateTime.Now;
                po.OrderBy = empid;
                po.Status = "Pending";
                //po.Employee = emp;
                e.PurchaseOrders.Add(po);
                e.SaveChanges();
                ts.Complete();
            }
            LUSSISEntities e2 = new LUSSISEntities();
            PurchaseOrder po1 = e2.PurchaseOrders.ToList().Last();
            int did = po1.PurchaseOrderID;
            return did;
        }
        //Irene
        public static void AddItemDetail(int oid, String sid, String ic, int qty)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                PurchaseOrderDetail po = new PurchaseOrderDetail();
                po.PurchaseOrderID = oid;
                po.Supplier = sid;
                Catalogue c = e.Catalogues.Where(x => x.ItemCode == ic).First();
                po.Catalogue = c;
                po.Qty = qty;
                e.PurchaseOrderDetails.Add(po);
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Irene
        public static void UpdateAmountByID(int oid, decimal amount)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                PurchaseOrder po = e.PurchaseOrders.Where(x => x.PurchaseOrderID == oid).First();
                po.Amount = amount;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Irene
        public static void UpdateQty(int id, int qty)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                DisbursementDetail newdd = e.DisbursementDetails.Where(x => x.DisbursementDetailID == id).First();                
                //newdd.ItemQty = qty;
                if(newdd.TempQty == null)
                {
                    newdd.TempQty = qty;
                }
                else
                {
                    newdd.TempQty = newdd.TempQty + qty;                    
                }
                
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Irene
        public static void UpdateVerify(int empid, int disbursementid, String itemcode)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (TransactionScope ts = new TransactionScope())
            {               
                Disbursement ditem = e.Disbursements.Where(x => x.DisbursementID == disbursementid).First();                
                List<DisbursementDetail> dd = e.DisbursementDetails.Where(x => x.DisbursementID == ditem.DisbursementID).ToList();
                List<RequisitionDetail> rd = e.RequisitionDetails.Where(x => x.DisbursementID == disbursementid && x.ItemCode == itemcode).ToList();
                
                ditem.DeptRep = empid;
                ditem.Disbursement_Date = DateTime.Now;
                
                Catalogue c;    //For update amount in Disbursement tables
                decimal total = 0; //For update amount in Disbursement tables

                for (int i = 0; i < dd.Count; i++)
                {
                    for (int j = 0; j < rd.Count; j++)
                    {
                        if (dd[i].ItemCode == rd[j].ItemCode)
                        {
                            ditem.Status = "Confirmed";
                            if(dd[i].TempQty != rd[j].ItemQty)
                            {
                                rd[j].OutstandingQty = rd[j].ItemQty - dd[i].TempQty;
                                rd[j].DisbursedQty = dd[i].TempQty;
                            }
                        }
                    }
                    //Update amount
                    String icode = dd[i].ItemCode;
                    c = e.Catalogues.Where(x => x.ItemCode == icode).First();
                    if(dd[i].TempQty!= null)
                    {
                        dd[i].Amount = c.AveragePrice * dd[i].TempQty;
                    }
                    else
                    {
                        dd[i].Amount = c.AveragePrice * dd[i].ItemQty;
                    }                    
                    total = total + (Decimal)dd[i].Amount;
                }
                ditem.Amount = total;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Irene
        public static List<RequisitionDetail> GetAllRequisitionDetail()
        {
            LUSSISEntities e = new LUSSISEntities();            
            List<RequisitionDetail> rdlist = e.RequisitionDetails.Distinct().ToList();
            return rdlist;
        }
        //Irene
        public static List<Requisition> GetApprovedRequisition()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Requisition> redetail = e.Requisitions.Where(x => x.Status == "Approved").ToList();
            return redetail;
        }
        //Irene
        public static List<RequisitionDetail> GetRequisitionDetailItemList(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Disbursement dItem = e.Disbursements.Where(x => x.DisbursementID == id).First();            
            List<RequisitionDetail> rdList = e.RequisitionDetails.Where(x => x.DisbursementID == dItem.DisbursementID).ToList();
            return rdList;
        }        
        //Irene
        public static List<Disbursement> GetDisbursementByCompleted()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Disbursement> listdisbursement = e.Disbursements.Where(x => x.Status == "Confirmed").ToList();
            return listdisbursement;
        }
        //Irene
        public static Disbursement GetDisbursementById(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Disbursement item = e.Disbursements.Where(x => x.DisbursementID == id).FirstOrDefault();
            return item;
        }
        //Irene
        public static List<DisbursementDetail> GetDisbursementDetailList(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            List<DisbursementDetail> listdisbursement = e.DisbursementDetails.Where(x => x.DisbursementID == id).ToList();
            return listdisbursement;
        }
        //Irene
        public static DisbursementDetail GetDisbursementDetail(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            DisbursementDetail disbursementItem = e.DisbursementDetails.Where(x => x.DisbursementDetailID == id).First();
            return disbursementItem;
        }
        //Irene
        public static List<Disbursement> GetDisbursementByIncomplete() //For Clerk
        {
            LUSSISEntities e = new LUSSISEntities();            
            List<Disbursement> listdisbursement = e.Disbursements.Where(x => x.Status =="Pending").ToList();
            return listdisbursement;
        }
        //Irene
        public static List<Department> GetAllDept()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Department> deptlist = e.Departments.ToList();
            return deptlist;
        }
        //Irene
        public static Department GetDeptInfo(String deptid)
        {
            LUSSISEntities e = new LUSSISEntities();
            Department d = e.Departments.Where(x => x.DepartmentID == deptid).First();
            return d;
        }
        //Irene
        public static List<Disbursement> GetDisbursementByIncomplete(String deptid) //For Rep
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Disbursement> listdisbursement = e.Disbursements.Where(x => x.DepartmentID == deptid && x.Status =="Pending").ToList();
            return listdisbursement;
        }        
        //Irene
        public static List<Disbursement> GetDisbursementByCompleted(String deptid) //For Rep
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Disbursement> listdisbursement = e.Disbursements.Where(x => x.DepartmentID == deptid && x.Status == "Confirmed").ToList();
            return listdisbursement;
        }
        //Irene
        public static void UpdateDisburseStockCard(String itemCode, String dept, int qty, int bal)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                StockCard s = new StockCard();
                s.ItemCode = itemCode;
                s.TransactionDate = DateTime.Now;
                s.Dept_Supplier_Adjt = "Department - " + dept;
                s.Qty = qty;
                s.AddorMinus = "-";
                s.Balance = bal;
                e.StockCards.Add(s);                               
                e.SaveChanges();
                ts.Complete();
            }            
        }
        //Irene
        public static void UpdateCatalogue(String itemCode, int qty)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (TransactionScope ts = new TransactionScope())
            {                 
                Catalogue c = e.Catalogues.Where(x => x.ItemCode == itemCode).First();
                c.CurrentStockQty = (int)(c.CurrentStockQty - qty);                
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Irene        
        public static void SendEmail(Email e)
        {
            try
            {
                MailMessage mail = new MailMessage();
                MailAddress sender = new MailAddress("admin-do-not-reply@lu.edu.sg");
                mail.From = sender;                
                mail.To.Add(e.Emailaddress);
                mail.Body = e.Body;
                mail.IsBodyHtml = true;
                mail.Subject = e.Subject;
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "localhost";
                smtpClient.Send(mail);
            }
            catch (Exception err)
            {
                String error = err.Message;
                Console.WriteLine(error);
            }

        }
        //Irene
        public static void CreateAdjustmentForDisbursement(String itemCode, int qty, int empid)
        {
            LUSSISEntities e = new LUSSISEntities();
            Catalogue c = e.Catalogues.Where(x => x.ItemCode == itemCode).First();
            using (TransactionScope ts = new TransactionScope())
            {
                AdjustmentVoucher adj = new AdjustmentVoucher();
                adj.ItemCode = itemCode;
                adj.Date = DateTime.Now;                
                adj.Qty = qty;
                adj.ClerkID = empid;
                adj.Status = "Pending";
                adj.AddorMinus = "-";
                adj.Reason = "Damaged";
                adj.Amount = c.AveragePrice * qty;
                e.AdjustmentVouchers.Add(adj);
                e.SaveChanges();
                ts.Complete();
            }
        }

        //Shi Hoay
        //Requisition
        public static List<Requisition> listRequisitionApproved()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Requisition> reqlist = e.Requisitions.Where(x => x.Status == "Approved" || x.Status == "Partially Fulfilled").ToList();
            return reqlist;
        }
        //Shi Hoay
        //public static Requisition getRequisitionByID(int id)
        //{
        //    LUSSISEntities e = new LUSSISEntities();
        //    Requisition r = e.Requisitions.Where(x => x.RequisitionID == id).First();
        //    return r;
        //}

        //Shi Hoay
        public static void UpdateRequisitionStatus(int id, string status)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                Requisition r = e.Requisitions.Where(x => x.RequisitionID == id).First();
                r.Status = status;
                try
                {
                    e.SaveChanges();
                    ts.Complete();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
            }
        }
        //Shi Hoay
        //Requisition Detail List
        public static List<RequisitionDetail> listRequisitionDetail()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<RequisitionDetail> reqDlist = e.RequisitionDetails.ToList();
            return reqDlist;
        }
        //Shi Hoay
        public static List<RequisitionDetail> listRequisitionDetailByID(int id)//RequistionID
        {
            LUSSISEntities e = new LUSSISEntities();
            List<RequisitionDetail> reqDlist = e.RequisitionDetails.Where(x => x.RequisitionID == id).ToList();
            return reqDlist;
        }
        //Shi Hoay
        public static List<RequisitionDetail> listRequisitionDetailByItemCode(string itemcode)//by ItemCode
        {
            LUSSISEntities e = new LUSSISEntities();
            List<RequisitionDetail> reqDlist = e.RequisitionDetails.Where(x => x.ItemCode == itemcode).ToList();
            //Additional Conditional
            List<RequisitionDetail> inlist = new List<RequisitionDetail>();
            Requisition r;
            foreach (var item in reqDlist)
            {
                r = e.Requisitions.Where(x => x.RequisitionID == item.RequisitionID).First();
                if (r.Status == "Approved" || r.Status == "Partially Fulfilled")
                {
                    inlist.Add(item);
                }
            }
            inlist.RemoveAll(x => x.OutstandingQty == 0);
            return inlist;
        }
        //Shi Hoay
        public static RequisitionDetail getRequisitionDetailItem(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            RequisitionDetail r = e.RequisitionDetails.Where(x => x.RequisitionDetailID == id).First();
            return r;
        }
        //Shi Hoay
        public static void UpdateRequisitionItem(int id, int n, int did)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                RequisitionDetail rc = e.RequisitionDetails.Where(x => x.RequisitionDetailID == id).First();
                rc.DisbursedQty += n;
                rc.OutstandingQty -= n;
                rc.DisbursementID = did;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        //Catalogue for listing items
        public static List<Catalogue> listCatalogue()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Catalogue> clist = e.Catalogues.ToList();
            return clist;
        }
        //Shi Hoay
        //Retrieval
        public static List<Retrieval> listRetrievalPending()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Retrieval> rplist = e.Retrievals.Where(x => x.Status == "Pending" || x.Status == "Retrieved" || x.Status == "Distributing").ToList();
            return rplist;
        }
        //Shi Hoay
        public static List<Retrieval> listRetrievalComplete()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<Retrieval> rclist = e.Retrievals.Where(x => x.Status == "Completed").ToList();
            return rclist;
        }
        //Shi Hoay
        public static Retrieval getRetrievalByID(int id)
        {
            LUSSISEntities e = new LUSSISEntities();
            Retrieval r = e.Retrievals.Where(x => x.RetrievalID == id).First();
            return r;
        }
        //Shi Hoay
        public static List<RetrievalDetail> listRetrievalDetail()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<RetrievalDetail> rdlist = e.RetrievalDetails.ToList();
            return rdlist;
        }
        //Shi Hoay
        public static List<RetrievalDetail> listRetrievalDetailByID(int id)//RetrievalID
        {
            LUSSISEntities e = new LUSSISEntities();
            List<RetrievalDetail> rdlistid = e.RetrievalDetails.Where(x => x.RetrievalID == id).ToList();
            return rdlistid;
        }
        //Shi Hoay
        public static RetrievalDetail getRetrievalItem(int id)//RetrievalDetailID - item
        {
            LUSSISEntities e = new LUSSISEntities();
            RetrievalDetail ritem = e.RetrievalDetails.Where(x => x.RetrievalDetailID == id).First();
            return ritem;
        }
        //Shi Hoay
        public static void UpdateRetrievalItemCollected(int id, int n)//when collected
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                RetrievalDetail rc = e.RetrievalDetails.Where(x => x.RetrievalDetailID == id).First();
                rc.ActualQty = n;
                rc.Status = "Collected";
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        public static void UpdateRetrievalItemDisbursed(string id) //after item is disbursed
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                String rid = id;
                for (int i = 0; i < rid.Split(',').Count(); i++)
                {
                    int rdid = Convert.ToInt32(rid.Split(',')[i]);
                    RetrievalDetail rc = e.RetrievalDetails.Where(x => x.RetrievalDetailID == rdid).First();
                    rc.Status = "Disbursed";
                }
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        public static void InsertAdjustmentVoucher(string itemcode, string pom, int qty, string reason, int empid)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                AdjustmentVoucher ad = new AdjustmentVoucher();
                Employee em = e.Employees.Where(x => x.EmployeeID == empid).First();
                ad.Employee = em;
                ad.Date = DateTime.Now;
                ad.ItemCode = itemcode;
                ad.Qty = qty;
                ad.AddorMinus = pom;
                ad.Reason = reason;
                ad.Status = "Pending";
                Catalogue c = e.Catalogues.Where(x => x.ItemCode == itemcode).First();
                ad.Amount = c.AveragePrice * qty;
                e.AdjustmentVouchers.Add(ad);
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        public static void UpdateRetrievalStatus(int id, string status)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                Retrieval r = e.Retrievals.Where(x => x.RetrievalID == id).First();
                r.Status = status;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        //Creation of Retrieval Form 
        public static int InsertRetrieval(int empid)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                Retrieval r = new Retrieval();
                Employee em = e.Employees.Where(x => x.EmployeeID == empid).First();
                r.ByClerkID = em.EmployeeID;
                r.Date = DateTime.Now;
                r.Status = "Pending";
                e.Retrievals.Add(r);
                e.SaveChanges();
                ts.Complete();
            }
            int rid = e.Retrievals.ToList().Last().RetrievalID;
            return rid;
        }
        //Shi Hoay
        public static void InsertRetrievalDetail(int id, string itemcode, int qty)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                RetrievalDetail rd = new RetrievalDetail();
                rd.RetrievalID = id;
                rd.ItemCode = itemcode;
                rd.QtyNeeded = qty;
                rd.ActualQty = '0';
                e.RetrievalDetails.Add(rd);
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        //Creation of Disbursement
        public static Disbursement getDisbursementByDept(string Dept)//find disbursement form by department
        {
            LUSSISEntities e = new LUSSISEntities();
            DateTime date = DateTime.Now.Date;
            Disbursement did = e.Disbursements.Where(x => x.DepartmentID == Dept).Where(y => y.Disbursement_Date == date).FirstOrDefault();
            return did;
        }
        //Shi Hoay
        public static DisbursementDetail getDisbursementDetailItem(int Did, string itemcode)//DisbursementID
        {
            LUSSISEntities e = new LUSSISEntities();
            DisbursementDetail dditem = e.DisbursementDetails.Where(x => x.DisbursementID == Did).Where(y => y.ItemCode == itemcode).FirstOrDefault();
            return dditem;
        }
        //Shi Hoay
        public static int InsertDisbursement(string Dept, int ReqId, int empid)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                Disbursement d = new Disbursement();
                Employee em = e.Employees.Where(x => x.EmployeeID == empid).First();
                d.Clerk = em.EmployeeID;
                d.Disbursement_Date = DateTime.Now.Date;
                Employee rep = e.Employees.Where(x => x.DepartmentID == Dept).Where(y => y.Authorization == "Representative").First();
                d.DeptRep = rep.EmployeeID;
                d.DepartmentID = Dept;
                d.Status = "Pending";
                e.Disbursements.Add(d);
                e.SaveChanges();
                ts.Complete();
            }
            int did = e.Disbursements.ToList().Last().DisbursementID;
            return did;
        }
        //Shi Hoay
        public static void UpdateDisbursementDetailItem(int id, int qty) //by DisbursementDetail ID
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                DisbursementDetail dd = e.DisbursementDetails.Where(x => x.DisbursementDetailID == id).First();
                dd.ItemQty += qty;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        public static void InsertDisbursementDetail(int id, string itemcode, int qty)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                DisbursementDetail dd = new DisbursementDetail();
                dd.DisbursementID = id;
                dd.ItemCode = itemcode;
                dd.ItemQty = qty;
                e.DisbursementDetails.Add(dd);
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        public static List<InterimRetrievalStockTable> listInterimRetrievalStock()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<InterimRetrievalStockTable> irsList = e.InterimRetrievalStockTables.ToList();
            return irsList;
        }
        //Shi Hoay
        public static void UpdateInterimRetrievalStockItemForRetrieval(string itemcode, int n)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                InterimRetrievalStockTable iitem = e.InterimRetrievalStockTables.Where(x => x.ItemCode == itemcode).First();
                iitem.WaitingQty -= n;
                e.SaveChanges();
                ts.Complete();
            }
        }
        //Shi Hoay
        public static void UpdateInterimRetrievalStockItemFromDisbursement(string itemcode, int n)
        {
            LUSSISEntities e = new LUSSISEntities();
            using (System.Transactions.TransactionScope ts = new TransactionScope())
            {
                InterimRetrievalStockTable iitem = e.InterimRetrievalStockTables.Where(x => x.ItemCode == itemcode).First();
                iitem.WaitingQty += n;
                e.SaveChanges();
                ts.Complete();
            }
        }        

        //Cao Jian
        public static List<ObjectForBar> GenerateRepByDep(string departmentIDs, string categoryids, string years, string months)
        {

            List<string> deplist = new List<string>();
            List<string> itemlist = new List<string>();
            List<string> yearlist = new List<string>();
            List<string> monthlist = new List<string>();
            //List<ArrayList> amountlist = new List<ArrayList>();
            List<ObjectForBar> amountlist = new List<ObjectForBar>();


            foreach (var depID in departmentIDs.Split(','))
                deplist.Add(depID);
            foreach (var i in categoryids.Split(','))
                itemlist.Add(i);
            foreach (var y in years.Split(','))
                yearlist.Add(y);
            foreach (var m in months.Split(','))
                monthlist.Add(m);


            foreach (var item1 in deplist)
            {
                foreach (var item2 in itemlist)
                {
                    foreach (var item3 in yearlist)
                    {
                        ArrayList temp = new ArrayList();
                        foreach (var item4 in monthlist)
                        {
                            ReqItem Qty = new ReqItem(item1, item2, item3, item4);
                            string cateID = Qty.categoryID;
                            Qty.getReqByDepid();
                            Qty.selectedReqByDate();
                            int? Amount = Qty.criteriaAmount();
                            temp.Add(Amount);
                        }

                        ObjectForBar tem = new Models.ObjectForBar(item2, temp);
                        amountlist.Add(tem);
                    }
                }
            }
            return amountlist;
        }
        //Cao Jian
        public static string getItemName(string item)
        {
            int itemID = int.Parse(item);
            LUSSISEntities lussis = new LUSSISEntities();
            string itemName = lussis.Categories.Where(x => x.CategoryID == itemID).Select(x => x.CategoryName).First();
            return itemName;
        }
        //Cao Jian
        public static string getDepName(string depID)
        {
            LUSSISEntities lussis = new LUSSISEntities();
            string DepName = lussis.Departments.Where(x => x.DepartmentID == depID).Select(x => x.DepartmentID).First();
            return DepName;
        }
        //Cao Jian
        public static List<ObjectForTable> creatTable(string categoryids)
        {

            LUSSISEntities lussis = new LUSSISEntities();
            List<string> itemlist = new List<string>();
            List<ObjectForTable> objects = new List<ObjectForTable>();

            foreach (var i in categoryids.Split(','))
                itemlist.Add(i);

            foreach (var item in itemlist)
            {
                int No = int.Parse(item);
                ArrayList nums = new ArrayList();

                string itemName = lussis.Categories.Where(x => x.CategoryID == No).Select(x => x.CategoryName).First();

                ArrayList amounts = new ArrayList();
                List<string> codes = lussis.Catalogues.Where(x => x.CategoryID == No).Select(x => x.ItemCode).ToList<String>();
                List<RequisitionDetail> reqs = lussis.RequisitionDetails.ToList<RequisitionDetail>();
                foreach (RequisitionDetail req in reqs)
                {
                    foreach (var code in codes)
                    {
                        if (req.ItemCode == code)
                        {
                            nums.Add(req.ItemQty);
                        }
                    }
                }
                ObjectForTable tem = new ObjectForTable(nums);
                tem.ItemName = itemName;
                //tem.caculate(nums);
                objects.Add(tem);
            }
            return objects;
        }        
    }
}