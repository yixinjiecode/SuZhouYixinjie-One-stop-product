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

import com.eos.das.entity.criteria.CriteriaType;
import com.eos.foundation.PageCond;

import org.gocom.components.nui.demo.newdataset.TDepartment;

/**
 * 完成部门信息的增删改查功能
 *
 * @author leijian (mailto:leijian@primeton.com)
 */
public interface ITDepartmentService{

	/**
	 *
	 * @param tDepartment TDepartment
	 */
	void addTDepartment(TDepartment tDepartment);

	/**
	 *
	 * @param tDepartments TDepartment[]
	 */
	void deleteTDepartment(TDepartment[] tDepartments);

	/**
	 *
	 * @param tDepartment TDepartment[]
	 */
	void getTDepartment(TDepartment tDepartment);

	/**
	 *
	 * @param criteria CriteriaType
	 * @param page PageCond
	 * @return TDepartment[]
	 */
	TDepartment[] queryTDepartments(CriteriaType criteriaType, PageCond pageCond, String sortFilder, String sortOrder);

	/**
	 *
	 * @param tDepartment TDepartment
	 */
	void updateTDepartment(TDepartment tDepartment);

	/**
	 *
	 * @param tDepartment TDepartment[]
	 */
	void updateTDepartments(TDepartment[] tDepartments);
	/**
	 *
	 * @param criteria CriteriaType
	 * @return int
	 */
	int countTDepartments(CriteriaType criteria);
}

