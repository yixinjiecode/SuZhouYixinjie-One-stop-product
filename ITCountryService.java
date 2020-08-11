/*
 * Copyright 2013 Primeton.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.gocom.components.nui.demo;

import com.eos.das.entity.criteria.CriteriaType;
import com.eos.foundation.PageCond;

import org.gocom.components.nui.demo.newdataset.TCountry;

/**
 * TODO 实现Demo中涉及到"国家"数据的增删改查功能
 *
 * @author huangct (mailto:huangct@primeton.com)
 */
public interface ITCountryService{

	/**
	 *
	 * @param tCountry TCountry
	 */
	void addTCountry(TCountry tCountry);

	/**
	 *
	 * @param tCountrys TCountry[]
	 */
	void deleteTCountry(TCountry[] tCountrys);

	/**
	 *
	 * @param tCountry TCountry[]
	 */
	void getTCountry(TCountry tCountry);
	
	/**
	 *
	 * @param criteria CriteriaType
	 * @return int
	 */
	int countTCountrys(CriteriaType criteria);
	

	/**
	 *
	 * @param criteria CriteriaType
	 * @param page PageCond
	 * @return TCountry[]
	 */
	TCountry[] queryTCountrys(CriteriaType criteria,
			PageCond page,String sortField,String sortOrder);

	/**
	 *
	 * @param tCountry TCountry[]
	 */
	void updateTCountry(TCountry tCountry);

}


