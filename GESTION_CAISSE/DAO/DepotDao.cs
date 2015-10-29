﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using GESTION_CAISSE.ENTITE;
using GESTION_CAISSE.TOOLS;

namespace GESTION_CAISSE.DAO
{
    class DepotDao
    {
        public static Depot getOneDepot(long id)
        {
            NpgsqlConnection con = Connexion.Connection();
            try
            {
                String search = "select * from yvs_base_depots where id = " + id + "";
                NpgsqlCommand Lcmd = new NpgsqlCommand(search, con);
                NpgsqlDataReader lect = Lcmd.ExecuteReader();
                Depot a = new Depot();
                if (lect.HasRows)
                {
                    while (lect.Read())
                    {
                        a.Id = Convert.ToInt64(lect["id"].ToString());
                        a.Abbreviation = lect["abbreviation"].ToString();
                        a.Designation = lect["designation"].ToString();
                        a.Emplacmenets = BLL.EmplacementBll.Liste("select * from yvs_base_emplacement_depot where depot = " + a.Id);
                        a.ArticlesAll = BLL.ArticleDepotBll.Liste("select a.id as id, a.emplacement as emplacement, a.article as article, a.mode_appro as mode_appro, a.mode_reappro as mode_reappro, a.stock_alert as stock_alert, a.stock_max as stock_max, a.stock_min as stock_min, a.quantite_stock as quantite_stock "
                                                                    + " from yvs_base_article_depot a inner join yvs_base_emplacement_depot e "
                                                                    + " on a.emplacement = e.id where e.depot = " + a.Id);
                        foreach (ArticleDepot d in a.ArticlesAll)
                        {
                            bool trouv = false;
                            foreach (ArticleDepot d_ in a.Articles)
                            {
                                if (d.Article.Equals(d_.Article))
                                {
                                    d.Id = d_.Id;
                                    d.Stock += d_.Stock;
                                    trouv = true;
                                    break;
                                }
                            }
                            if (!trouv)
                            {
                                a.Id = a.Articles.Count;
                                a.Articles.Add(d);
                            }
                            else
                            {
                                a.Articles[a.Articles.FindIndex(x => x.Id == d.Id)] = d;
                            }
                        }
                        a.Update = true;
                    }
                    lect.Close();
                }
                return a;
            }
            catch (NpgsqlException e)
            {
                Messages.Exception(e);
                return null;
            }
            finally
            {
                Connexion.Deconnection(con);
            }
        }

        private static long getCurrent()
        {
            NpgsqlConnection con = Connexion.Connection();
            try
            {
                String search = "select id from yvs_base_depots order by id desc limit 1";
                NpgsqlCommand Lcmd = new NpgsqlCommand(search, con);
                NpgsqlDataReader lect = Lcmd.ExecuteReader();
                long id = 0;
                if (lect.HasRows)
                {
                    while (lect.Read())
                    {
                        id = Convert.ToInt64(lect["id"].ToString());
                    }
                    lect.Close();
                }
                return id;
            }
            catch (NpgsqlException e)
            {
                Messages.Exception(e);
                return 0;
            }
            finally
            {
                Connexion.Deconnection(con);
            }
        }

        public static Depot getAjoutDepot(Depot a)
        {
            NpgsqlConnection con = Connexion.Connection();
            try
            {
                string insert = "";
                NpgsqlCommand cmd = new NpgsqlCommand(insert, con);
                cmd.ExecuteNonQuery();
                a.Id = getCurrent();
                return a;
            }
            catch
            {
                return null;
            }
            finally
            {
                Connexion.Deconnection(con);
            }
        }

        public static bool getUpdateDepot(Depot a)
        {
            NpgsqlConnection con = Connexion.Connection();
            try
            {
                string update = "";
                NpgsqlCommand Ucmd = new NpgsqlCommand(update, con);
                Ucmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Messages.Exception(e);
                return false;
            }
            finally
            {
                Connexion.Deconnection(con);
            }
        }

        public static bool getDeleteDepot(long id)
        {
            NpgsqlConnection con = Connexion.Connection();
            try
            {
                string delete = "";
                NpgsqlCommand Ucmd = new NpgsqlCommand(delete, con);
                Ucmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Messages.Exception(e);
                return false;
            }
            finally
            {
                Connexion.Deconnection(con);
            }
        }

        public static List<Depot> getListDepot(String query)
        {
            NpgsqlConnection con = Connexion.Connection();
            try
            {
                List<Depot> l = new List<Depot>();
                NpgsqlCommand Lcmd = new NpgsqlCommand(query, con);
                NpgsqlDataReader lect = Lcmd.ExecuteReader();
                if (lect.HasRows)
                {
                    while (lect.Read())
                    {
                        Depot a = new Depot();
                        a.Id = Convert.ToInt64(lect["id"].ToString());
                        a.Abbreviation = lect["abbreviation"].ToString();
                        a.Designation = lect["designation"].ToString();
                        a.Update = true;
                        l.Add(a);
                    }
                    lect.Close();
                }
                return l;
            }
            catch (NpgsqlException e)
            {
                Messages.Exception(e);
                return null;
            }
            finally
            {
                Connexion.Deconnection(con);
            }
        }
    }
}
