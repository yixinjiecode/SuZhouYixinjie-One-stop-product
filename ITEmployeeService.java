/*
 * Copyright 2013 Primeton Technologies Ltd.
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

import org.gocom.components.nui.demo.newdataset.TEmployee;

import com.eos.das.entity.criteria.CriteriaType;
import com.eos.foundation.PageCond;

/**
 *	完成员工信息的增删改查功能
 *
 * @author leijian (mailto:leijian@primeton.com)
 */
public interface ITEmployeeService{

	/**
	 *
	 * @param tEmployee TEmployee
	 */
	void addTEmployee(TEmployee tEmployee);

	/**
	 *
	 * @param tEmployees TEmployee[]
	 */
	void deleteTEmployee(TEmployee[] tEmployees);

	/**
	 *
	 * @param tEmployee TEmployee
	 */
	void getTEmployee(TEmployee tEmployee);

	/**
	 *
	 * @param criteriaType
	 * @param pageCond
	 * @return
	 */
	TEmployee[] queryTEmployees(CriteriaType criteriaType, PageCond pageCond, String sortField, String sortOrder);

	/**
	 *
	 * @param tEmployee TEmployee[]
	 */
	void updateTEmployee(TEmployee tEmployee);
	
	/**
	 *
	 * @param criteria CriteriaType
	 * @return int
	 */
	int countEmployees(CriteriaType criteria);
	
	/**
	 *
	 * @param employees TEmployee[]
	 */
	void saveEmployees(TEmployee[] employees);
	
	/**
	 *
	 * @param employees TEmployee
	 */
	void deleteTEmployee(TEmployee employee);
	
}


